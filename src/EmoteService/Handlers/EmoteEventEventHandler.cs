using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Events;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers;

public class EmoteEventEventHandler : BaseEventHandlerWithDb<EmoteEventPayload, EmoteServiceContext>
{
    public EmoteEventEventHandler(ILoggerFactory loggerFactory, EmoteServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(EmoteEventPayload payload)
    {
        var emoteValue = Emote.Parse(payload.EmoteId);
        if (!await IsSupportedEmoteAsync(payload.GuildId, emoteValue))
            return;

        var entity = await GetEntityAsync(payload.GuildId, payload.UserId, emoteValue);
        if (entity is null)
        {
            if (payload.IsIncrement)
                entity = await CreateEntityAsync(payload.GuildId, payload.UserId, emoteValue);
            else
                return;
        }

        if (payload.IsIncrement)
        {
            if (entity.FirstOccurence == DateTime.MinValue)
                entity.FirstOccurence = payload.EventCreatedAt;

            entity.LastOccurence = payload.EventCreatedAt;
            entity.UseCount++;
        }
        else
        {
            entity.UseCount--;

            if (entity.UseCount <= 0)
                DbContext.Remove(entity);
        }

        using (CreateCounter("Database"))
            await DbContext.SaveChangesAsync();
    }

    private void ValidationFailed(string message)
        => Logger.LogWarning(new EventId(2, "ValidationFailed_PublishAudit"), "{message}", message);

    private async Task<bool> IsSupportedEmoteAsync(string guildId, Emote emote)
    {
        bool isSupported;
        using (CreateCounter("Database"))
            isSupported = await DbContext.EmoteDefinitions.Where(o => o.GuildId == guildId).WithEmoteQuery(emote).AnyAsync();

        if (!isSupported)
            ValidationFailed($"Unsupported emote {emote}");
        return isSupported;
    }

    private async Task<EmoteUserStatItem?> GetEntityAsync(string guildId, string userId, Emote emote)
    {
        using (CreateCounter("Database"))
            return await DbContext.EmoteUserStatItems.Where(o => o.GuildId == guildId && o.UserId == userId).WithEmoteQuery(emote).FirstOrDefaultAsync();
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
