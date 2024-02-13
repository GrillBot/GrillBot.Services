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
        var request = (TransactionRequest)Parameters[0]!;
        var author = (await Repository.User.FindUserAsync(request.GuildId, request.MessageInfo.AuthorId))!;
        var reactionUser = request.ReactionInfo is null ? null : await Repository.User.FindUserAsync(request.GuildId, request.ReactionInfo.UserId);
        var channel = (await Repository.Channel.FindChannelAsync(request.GuildId, request.ChannelId))!;

        if (!await CanCreateTransactionAsync(author, reactionUser, channel, request))
            return new ApiResult(StatusCodes.Status406NotAcceptable);

        var transaction = new Transaction
        {
            UserId = (reactionUser ?? author).Id,
            Value = ComputeValue(request),
            GuildId = request.GuildId,
            MessageId = request.MessageInfo.Id,
            ReactionId = request.ReactionInfo?.GetReactionId() ?? ""
        };

        await Repository.AddAsync(transaction);

        if (reactionUser != null)
        {
            if (request.ReactionInfo?.IsBurst == true)
                reactionUser.LastSuperReactionIncrement = transaction.CreatedAt;
            else
                reactionUser.LastReactionIncrement = transaction.CreatedAt;

            reactionUser.PendingRecalculation = true;
        }
        else
        {
            author.LastMessageIncrement = transaction.CreatedAt;
            author.PendingRecalculation = true;
        }

        await Repository.CommitAsync();
        return ApiResult.Ok();
    }

    private async Task<bool> CanCreateTransactionAsync(User author, User? reactionUser, Channel channel, TransactionRequest request)
    {
        var user = reactionUser ?? author;

        if (!user.IsUser || user.PointsDisabled) return false;
        if (channel.IsDeleted || channel.PointsDisabled) return false;
        if (request.MessageInfo.MessageType is MessageType.ApplicationCommand or MessageType.ContextMenuCommand) return false;
        if (author.Id == reactionUser?.Id) return false;
        if (!CheckCooldown(user, request)) return false;

        var minLength = Options.Message.GetConfigurationValue<int>("MinLength");
        if (request.MessageInfo.ContentLength < minLength) return false;

        return await IsUniqueRequestAsync(request);
    }

    private bool CheckCooldown(User user, TransactionRequest request)
    {
        var incrementType = request.GetIncrementType();

        var cooldown = incrementType switch
        {
            Enums.IncrementType.Reaction => Options.Reactions.Cooldown,
            Enums.IncrementType.SuperReaction => Options.SuperReactions.Cooldown,
            _ => Options.Message.Cooldown
        };

        var lastIncrement = incrementType switch
        {
            Enums.IncrementType.Reaction => user.LastReactionIncrement,
            Enums.IncrementType.SuperReaction => user.LastSuperReactionIncrement,
            _ => user.LastMessageIncrement
        };

        return lastIncrement == null || lastIncrement.Value.AddSeconds(cooldown) <= DateTime.UtcNow;
    }

    private int ComputeValue(TransactionRequest request)
    {
        var incrementType = request.GetIncrementType();

        var config = incrementType switch
        {
            Enums.IncrementType.Reaction => Options.Reactions,
            Enums.IncrementType.SuperReaction => Options.SuperReactions,
            _ => Options.Message
        };

        return RandomManager.GetNext("Points", config.Min, config.Max);
    }

    private async Task<bool> IsUniqueRequestAsync(TransactionRequest request)
    {
        var reactionId = request.ReactionInfo?.GetReactionId() ?? "";
        var userId = request.ReactionInfo?.UserId ?? request.MessageInfo.AuthorId;

        var exists = await Repository.Transaction.ExistsTransactionAsync(request.GuildId, request.MessageInfo.Id, userId, reactionId);
        return !exists;
    }
}
