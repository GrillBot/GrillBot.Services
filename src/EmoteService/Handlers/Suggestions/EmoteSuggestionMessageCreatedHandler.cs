using EmoteService.Core.Entity;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionMessageCreatedHandler(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : BaseEventHandlerWithDb<EmoteSuggestionMessageCreatedPayload, EmoteServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteSuggestionMessageCreatedPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        var suggestion = await DbContext.EmoteSuggestions.FirstOrDefaultAsync(o => o.Id == message.SuggestionId);
        if (suggestion is null)
            return RabbitConsumptionResult.Success;

        suggestion.SuggestionMessageId = message.MessageId;

        return await DbContext.SaveChangesAsync() > 0 ?
            RabbitConsumptionResult.Success :
            RabbitConsumptionResult.Reject;
    }
}
