using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Events;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace EmoteService.Handlers;

public class EmoteEventEventHandler(IServiceProvider serviceProvider)
    : BaseEventHandlerWithDb<EmoteEventPayload, EmoteServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteEventPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var emoteValue = Emote.Parse(message.EmoteId);
        if (message.IsIncrement && !await IsSupportedEmoteAsync(emoteValue))
            return RabbitConsumptionResult.Success;

        var entity = await GetEntityAsync(message.GuildId, message.UserId, emoteValue);
        if (entity is null)
        {
            if (message.IsIncrement)
                entity = await CreateEntityAsync(message.GuildId, message.UserId, emoteValue);
            else
                return RabbitConsumptionResult.Success;
        }

        if (message.IsIncrement)
        {
            if (entity.FirstOccurence == DateTime.MinValue)
                entity.FirstOccurence = message.EventCreatedAt;

            entity.LastOccurence = message.EventCreatedAt;
            entity.UseCount++;
        }
        else
        {
            entity.UseCount--;

            if (entity.UseCount <= 0)
                DbContext.Remove(entity);
        }

        await ContextHelper.SaveChangesAsync();
        return RabbitConsumptionResult.Success;
    }

    private void ValidationFailed(string message)
        => Logger.LogWarning(new EventId(2, "ValidationFailed_PublishAudit"), "{message}", message);

    private async Task<bool> IsSupportedEmoteAsync(Emote emote)
    {
        var query = DbContext.EmoteDefinitions.WithEmoteQuery(emote);
        var isSupported = await ContextHelper.IsAnyAsync(query);

        if (!isSupported)
            ValidationFailed($"Unsupported emote {emote}");
        return isSupported;
    }

    private async Task<EmoteUserStatItem?> GetEntityAsync(string guildId, string userId, Emote emote)
    {
        var query = DbContext.EmoteUserStatItems.Where(o => o.GuildId == guildId && o.UserId == userId).WithEmoteQuery(emote);
        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }

    private async Task<EmoteUserStatItem> CreateEntityAsync(string guildId, string userId, Emote emote)
    {
        var entity = new EmoteUserStatItem
        {
            GuildId = guildId,
            EmoteId = emote.Id.ToString(),
            EmoteIsAnimated = emote.Animated,
            UserId = userId,
            EmoteName = emote.Name
        };

        await DbContext.AddAsync(entity);
        return entity;
    }
}
