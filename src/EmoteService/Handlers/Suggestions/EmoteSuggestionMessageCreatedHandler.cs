using EmoteService.Core.Entity;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionMessageCreatedHandler(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<EmoteSuggestionMessageCreatedPayload, EmoteServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        EmoteSuggestionMessageCreatedPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        var query = DbContext.EmoteSuggestions.Where(o => o.Id == message.SuggestionId);
        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(query, cancellationToken);
        if (suggestion is null)
            return RabbitConsumptionResult.Success;

        suggestion.SuggestionMessageId = message.MessageId;

        await ContextHelper.SaveChangesAsync(cancellationToken);
        return RabbitConsumptionResult.Success;
    }
}
