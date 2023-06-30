using Discord;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Random;
using Microsoft.Extensions.Options;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class CreateTransactionAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }
    private AppOptions Options { get; }
    private IRandomManager RandomManager { get; }

    public CreateTransactionAction(PointsServiceRepository repository, IOptions<AppOptions> appOptions, IRandomManager randomManager)
    {
        Repository = repository;
        Options = appOptions.Value;
        RandomManager = randomManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (TransactionRequest)Parameters.First()!;
        var author = (await Repository.User.FindUserAsync(request.GuildId, request.MessageInfo.AuthorId))!;
        var reactionUser = request.ReactionInfo is null ? null : await Repository.User.FindUserAsync(request.GuildId, request.ReactionInfo.UserId);
        var channel = (await Repository.Channel.FindChannelAsync(request.GuildId, request.ChannelId))!;

        if (!await CanCreateTransactionAsync(author, reactionUser, channel, request))
            return new ApiResult(StatusCodes.Status406NotAcceptable);

        var transaction = new Transaction
        {
            UserId = (reactionUser ?? author).Id,
            Value = ComputeValue(reactionUser != null),
            GuildId = request.GuildId,
            MessageId = request.MessageInfo.Id,
            ReactionId = request.ReactionInfo?.GetReactionId() ?? ""
        };

        await Repository.AddAsync(transaction);

        if (reactionUser != null)
        {
            reactionUser.LastReactionIncrement = transaction.CreatedAt;
            reactionUser.PendingRecalculation = true;
        }
        else
        {
            author.LastMessageIncrement = transaction.CreatedAt;
            author.PendingRecalculation = true;
        }

        await Repository.CommitAsync();
        return new ApiResult(StatusCodes.Status200OK);
    }

    private async Task<bool> CanCreateTransactionAsync(User author, User? reactionUser, Channel channel, TransactionRequest request)
    {
        var user = reactionUser ?? author;

        if (!user.IsUser || user.PointsDisabled) return false;
        if (channel.IsDeleted || channel.PointsDisabled) return false;
        if (request.MessageInfo.ContentLength < Options.MinimalMessageLength) return false;
        if (request.MessageInfo.MessageType is MessageType.ApplicationCommand or MessageType.ContextMenuCommand) return false;
        if (author.Id == reactionUser?.Id) return false;
        if (!CheckCooldown(user, reactionUser != null)) return false;

        return await IsUniqueRequestAsync(request);
    }

    private bool CheckCooldown(User user, bool isReaction)
    {
        var cooldown = isReaction ? Options.ReactionCooldown : Options.MessageCooldown;
        var lastIncrement = isReaction ? user.LastReactionIncrement : user.LastMessageIncrement;

        return lastIncrement == null || lastIncrement.Value.AddSeconds(cooldown) <= DateTime.UtcNow;
    }

    private int ComputeValue(bool isReaction)
    {
        var min = isReaction ? Options.ReactionPointsMin : Options.MessagePointsMin;
        var max = isReaction ? Options.ReactionPointsMax : Options.MessagePointsMax;

        return RandomManager.GetNext("Points", min, max);
    }

    private async Task<bool> IsUniqueRequestAsync(TransactionRequest request)
    {
        var reactionId = request.ReactionInfo?.GetReactionId() ?? "";
        var userId = request.ReactionInfo?.UserId ?? request.MessageInfo.AuthorId;

        var exists = await Repository.Transaction.ExistsTransactionAsync(request.GuildId, request.MessageInfo.Id, userId, reactionId);
        return !exists;
    }
}
