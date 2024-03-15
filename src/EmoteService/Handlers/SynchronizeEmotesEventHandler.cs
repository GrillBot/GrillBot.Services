using Discord;
using EmoteService.Core.Entity;
using EmoteService.Models.Events;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace EmoteService.Handlers;

public class SynchronizeEmotesEventHandler : BaseEventHandlerWithDb<SynchronizeEmotesPayload, EmoteServiceContext>
{
    public SynchronizeEmotesEventHandler(ILoggerFactory loggerFactory, EmoteServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(SynchronizeEmotesPayload payload)
    {
        await ClearEmotesAsync(payload.GuildId);
        await InsertEmotesAsync(payload.GuildId, payload.Emotes);
    }

    private async Task ClearEmotesAsync(string guildId)
    {
        var query = DbContext.EmoteDefinitions.Where(o => o.GuildId == guildId);
        var emotes = await ContextHelper.ReadEntitiesAsync(query);
        if (emotes.Count == 0)
            return;

        DbContext.RemoveRange(emotes);
        await ContextHelper.SaveChagesAsync();
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

        await DbContext.AddRangeAsync(emoteEntities);
        await ContextHelper.SaveChagesAsync();
    }
}
