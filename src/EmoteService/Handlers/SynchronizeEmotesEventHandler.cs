using Discord;
using EmoteService.Core.Entity;
using EmoteService.Models.Events;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers;

public class SynchronizeEmotesEventHandler : BaseEventHandlerWithDb<SynchronizeEmotesEvent, EmoteServiceContext>
{
    public SynchronizeEmotesEventHandler(ILoggerFactory loggerFactory, EmoteServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(SynchronizeEmotesEvent payload)
    {
        await ClearEmotesAsync(payload.GuildId);
        await InsertEmotesAsync(payload.GuildId, payload.Emotes);
    }

    private async Task ClearEmotesAsync(string guildId)
    {
        List<EmoteDefinition> emotes;
        using (CreateCounter("Database"))
            emotes = await DbContext.EmoteDefinitions.Where(o => o.GuildId == guildId).ToListAsync();

        if (emotes.Count == 0)
            return;

        using (CreateCounter("Database"))
        {
            DbContext.RemoveRange(emotes);
            await DbContext.SaveChangesAsync();
        }
    }

    private async Task InsertEmotesAsync(string guildId, List<string> emotes)
    {
        if (emotes.Count == 0)
            return;

        var emoteEntities = emotes
            .Select(Emote.Parse)
            .Select(e => new EmoteDefinition
            {
                EmoteId = e.Id.ToString(),
                EmoteIsAnimated = e.Animated,
                EmoteName = e.Name,
                GuildId = guildId
            });

        using (CreateCounter("Database"))
        {
            await DbContext.AddRangeAsync(emoteEntities);
            await DbContext.SaveChangesAsync();
        }
    }
}
