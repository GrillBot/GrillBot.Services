﻿using Discord;
using EmoteService.Core.Entity;
using EmoteService.Models.Events;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace EmoteService.Handlers;

public class SynchronizeEmotesEventHandler(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : BaseEventHandlerWithDb<SynchronizeEmotesPayload, EmoteServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(SynchronizeEmotesPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        await ClearEmotesAsync(message.GuildId);
        await InsertEmotesAsync(message.GuildId, message.Emotes);

        return RabbitConsumptionResult.Success;
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
