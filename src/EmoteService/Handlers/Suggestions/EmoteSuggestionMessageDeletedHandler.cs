using EmoteService.Core.Entity;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionMessageDeletedHandler(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : BaseEventHandlerWithDb<EmoteSuggestionMessageDeletedPayload, EmoteServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteSuggestionMessageDeletedPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        ArgumentOutOfRangeException.ThrowIfZero(message.GuildId);
        ArgumentOutOfRangeException.ThrowIfZero(message.MessageId);

        var query = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession)
            .Where(o => o.GuildId == message.GuildId && o.SuggestionMessageId == message.MessageId);

        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        if (suggestion == null)
            return RabbitConsumptionResult.Success;

        suggestion.ApprovedForVote = false;

        if (suggestion.VoteSession is not null)
            suggestion.VoteSession.KilledAtUtc = DateTime.UtcNow;

        await ContextHelper.SaveChagesAsync();
        return RabbitConsumptionResult.Success;
    }
}
