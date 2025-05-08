using EmoteService.Core.Entity;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionVoteMessageCreatedHandler(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : EmoteSuggestionHandlerBase<EmoteSuggestionVoteMessageCreatedPayload>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteSuggestionVoteMessageCreatedPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        var suggestionQuery = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession)
            .Where(o => o.Id == message.SuggestionId && o.VoteSession != null);

        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(suggestionQuery);
        if (suggestion == null)
            return RabbitConsumptionResult.Reject;

        suggestion.VoteSession!.VoteMessageId = message.MessageId;
        await DbContext.SaveChangesAsync();

        return RabbitConsumptionResult.Success;
    }
}
