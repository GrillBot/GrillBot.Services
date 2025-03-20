using Discord;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Managers.Random;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers;

public class CreateTransactionEventHandler(
    ILoggerFactory loggerFactory,
    PointsServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher,
    IOptions<AppOptions> _options,
    IRandomManager _randomManager
) : CreateTransactionBaseEventHandler<CreateTransactionPayload>(loggerFactory, dbContext, counterManager, publisher)
{
    public override string QueueName => "CreateTransaction";

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(CreateTransactionPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var author = await FindOrCreateUserAsync(message.GuildId, message.Message.AuthorId);
        var reactionUser = message.Reaction is null ? null : await FindOrCreateUserAsync(message.GuildId, message.Reaction.UserId);
        var channel = await FindOrCreateChannelAsync(message);

        if (!await CanCreateTransactionAsync(message, author, reactionUser, channel))
            return RabbitConsumptionResult.Success;

        var userId = (reactionUser ?? author)!.Id;
        var transaction = new Transaction
        {
            CreatedAt = message.CreatedAtUtc,
            GuildId = message.GuildId,
            UserId = userId,
            MessageId = message.Message.Id,
            ReactionId = message.Reaction?.GetReactionId() ?? "",
            Value = ComputePoints(message)
        };

        UpdateIncrementTime(author!, reactionUser, transaction, message.Reaction?.IsBurst ?? false);

        await CommitTransactionAsync(transaction);
        await EnqueueUserForRecalculationAsync(message.GuildId, userId);
        return RabbitConsumptionResult.Success;
    }

    private async Task<Channel> FindOrCreateChannelAsync(CreateTransactionPayload message)
    {
        var channelQuery = DbContext.Channels.Where(o => o.GuildId == message.GuildId && o.Id == message.ChannelId);
        var channel = await ContextHelper.ReadFirstOrDefaultEntityAsync(channelQuery);

        if (channel is null)
        {
            channel = new Channel
            {
                GuildId = message.GuildId,
                Id = message.ChannelId
            };

            await DbContext.AddAsync(channel);
        }

        return channel;
    }

    private async Task<bool> CanCreateTransactionAsync(CreateTransactionPayload message, User author, User? reactionUser, Channel channel)
    {
        // User validation
        var user = reactionUser ?? author;
        if (!user.IsUser)
            return await ValidationFailedAsync(message, message.ChannelId, "Unable to give points to the bot.");
        if (user.PointsDisabled)
            return await ValidationFailedAsync(message, message.ChannelId, "Target user have disabled points.", true);

        // Channel validation
        if (channel.IsDeleted)
            return await ValidationFailedAsync(message, message.ChannelId, "Unable to give points to the deleted channel.");
        if (channel.PointsDisabled)
            return await ValidationFailedAsync(message, message.ChannelId, "Target channel have disabled points.", true);

        // Message validation
        if (message.Message.MessageType is MessageType.ApplicationCommand or MessageType.ContextMenuCommand)
            return await ValidationFailedAsync(message, message.ChannelId, "Unable to give points to the command.");
        if (author.Id == reactionUser?.Id)
            return await ValidationFailedAsync(message, message.ChannelId, "Unable to give points to when reaction and message author have same owner.");
        if (!CheckCooldown(user, message))
            return await ValidationFailedAsync(message, message.ChannelId, "Unable to give points, applied cooldown policy.", true);
        if (message.Message.ContentLength < _options.Value.Message.GetConfigurationValue<int>("MinLength"))
            return await ValidationFailedAsync(message, message.ChannelId, "Unable to give points, applied message length policy.", true);

        // Transaction validation
        if (await CheckTransactionExistsAsync(message))
            return await ValidationFailedAsync(message, message.ChannelId, "Unable to give points, duplicate transcation.", true);

        return true;
    }

    private int ComputePoints(CreateTransactionPayload message)
    {
        var config = GetConfig(message);
        return _randomManager.GetNext("Points", config.Min, config.Max);
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

    private IncrementOptions GetConfig(CreateTransactionPayload message)
    {
        return message.GetIncrementType() switch
        {
            Enums.IncrementType.Reaction => _options.Value.Reactions,
            Enums.IncrementType.SuperReaction => _options.Value.SuperReactions,
            _ => _options.Value.Message
        };
    }

    private bool CheckCooldown(User user, CreateTransactionPayload message)
    {
        var lastIncrement = message.GetIncrementType() switch
        {
            Enums.IncrementType.Reaction => user.LastReactionIncrement,
            Enums.IncrementType.SuperReaction => user.LastSuperReactionIncrement,
            _ => user.LastMessageIncrement
        };

        var config = GetConfig(message);
        return lastIncrement is null || lastIncrement.Value.AddSeconds(config.Cooldown) <= message.CreatedAtUtc;
    }

    private async Task<bool> CheckTransactionExistsAsync(CreateTransactionPayload message)
    {
        var reactionId = message.Reaction?.GetReactionId() ?? "";
        var userId = message.Reaction?.UserId ?? message.Message.AuthorId;

        var existsQuery = DbContext.Transactions.AsNoTracking()
            .Where(o => o.GuildId == message.GuildId && o.MessageId == message.Message.Id && o.UserId == userId && o.ReactionId == reactionId);
        return await ContextHelper.IsAnyAsync(existsQuery);
    }
}
