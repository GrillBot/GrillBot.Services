using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Managers.Random;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers;

public class CreateTransactionEventHandler : CreateTransactionBaseEventHandler<CreateTransactionPayload>
{
    private AppOptions Options { get; }
    private IRandomManager RandomManager { get; }

    public CreateTransactionEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher,
        IOptions<AppOptions> appOptions, IRandomManager randomManager) : base(loggerFactory, dbContext, counterManager, publisher)
    {
        Options = appOptions.Value;
        RandomManager = randomManager;
    }

    protected override async Task HandleInternalAsync(CreateTransactionPayload payload)
    {
        var author = await FindOrCreateUserAsync(payload.GuildId, payload.Message.AuthorId);
        var reactionUser = payload.Reaction is null ? null : await FindOrCreateUserAsync(payload.GuildId, payload.Reaction.UserId);
        var channel = await FindOrCreateChannelAsync(payload);

        if (!await CanCreateTransactionAsync(payload, author, reactionUser, channel))
            return;

        var userId = (reactionUser ?? author)!.Id;
        var transaction = new Transaction
        {
            CreatedAt = payload.CreatedAtUtc,
            GuildId = payload.GuildId,
            UserId = userId,
            MessageId = payload.Message.Id,
            ReactionId = payload.Reaction?.GetReactionId() ?? "",
            Value = ComputePoints(payload)
        };

        UpdateIncrementTime(author!, reactionUser, transaction, payload.Reaction?.IsBurst ?? false);

        await CommitTransactionAsync(transaction);
        await EnqueueUserForRecalculationAsync(payload.GuildId, userId);
    }

    private async Task<Channel> FindOrCreateChannelAsync(CreateTransactionPayload payload)
    {
        var channelQuery = DbContext.Channels.Where(o => o.GuildId == payload.GuildId && o.Id == payload.ChannelId);
        var channel = await ContextHelper.ReadFirstOrDefaultEntityAsync(channelQuery);

        if (channel is null)
        {
            channel = new Channel
            {
                GuildId = payload.GuildId,
                Id = payload.ChannelId
            };

            await DbContext.AddAsync(channel);
        }

        return channel;
    }

    private async Task<bool> CanCreateTransactionAsync(CreateTransactionPayload payload, User author, User? reactionUser, Channel channel)
    {
        // User validation
        var user = reactionUser ?? author;
        if (!user.IsUser)
            return ValidationFailed("Unable to give points to the bot.");
        if (user.PointsDisabled)
            return ValidationFailed("Target user have disabled points.", true);

        // Channel validation
        if (channel.IsDeleted)
            return ValidationFailed("Unable to give points to the deleted channel.");
        if (channel.PointsDisabled)
            return ValidationFailed("Target channel have disabled points.", true);

        // Message validation
        if (payload.Message.MessageType is MessageType.ApplicationCommand or MessageType.ContextMenuCommand)
            return ValidationFailed("Unable to give points to the command.");
        if (author.Id == reactionUser?.Id)
            return ValidationFailed("Unable to give points to when reaction and message author have same owner.");
        if (!CheckCooldown(user, payload))
            return ValidationFailed("Unable to give points, applied cooldown policy.", true);
        if (payload.Message.ContentLength < Options.Message.GetConfigurationValue<int>("MinLength"))
            return ValidationFailed("Unable to give points, applied message length policy.", true);

        // Transaction validation
        if (await CheckTransactionExistsAsync(payload))
            return ValidationFailed("Unable to give points, duplicate transcation.", true);

        return true;
    }

    private int ComputePoints(CreateTransactionPayload payload)
    {
        var config = GetConfig(payload);
        return RandomManager.GetNext("Points", config.Min, config.Max);
    }

    private static void UpdateIncrementTime(User author, User? reactionUser, Transaction transaction, bool isBurstReaction)
    {
        if (reactionUser is not null)
        {
            if (isBurstReaction)
                reactionUser.LastSuperReactionIncrement = transaction.CreatedAt;
            else
                reactionUser.LastReactionIncrement = transaction.CreatedAt;
        }
        else
        {
            author.LastMessageIncrement = transaction.CreatedAt;
        }
    }

    private IncrementOptions GetConfig(CreateTransactionPayload payload)
    {
        return payload.GetIncrementType() switch
        {
            Enums.IncrementType.Reaction => Options.Reactions,
            Enums.IncrementType.SuperReaction => Options.SuperReactions,
            _ => Options.Message
        };
    }

    private bool CheckCooldown(User user, CreateTransactionPayload payload)
    {
        var lastIncrement = payload.GetIncrementType() switch
        {
            Enums.IncrementType.Reaction => user.LastReactionIncrement,
            Enums.IncrementType.SuperReaction => user.LastSuperReactionIncrement,
            _ => user.LastMessageIncrement
        };

        var config = GetConfig(payload);
        return lastIncrement is null || lastIncrement.Value.AddSeconds(config.Cooldown) <= payload.CreatedAtUtc;
    }

    private async Task<bool> CheckTransactionExistsAsync(CreateTransactionPayload payload)
    {
        var reactionId = payload.Reaction?.GetReactionId() ?? "";
        var userId = payload.Reaction?.UserId ?? payload.Message.AuthorId;

        var existsQuery = DbContext.Transactions.AsNoTracking()
            .Where(o => o.GuildId == payload.GuildId && o.MessageId == payload.Message.Id && o.UserId == userId && o.ReactionId == reactionId);
        return await ContextHelper.IsAnyAsync(existsQuery);
    }
}
