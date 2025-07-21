using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionVoteMessageCreatedHandler(IServiceProvider serviceProvider) : EmoteSuggestionHandlerBase<EmoteSuggestionVoteMessageCreatedPayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        EmoteSuggestionVoteMessageCreatedPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        var suggestionQuery = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession)
            .Where(o => o.Id == message.SuggestionId && o.VoteSession != null);

        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(suggestionQuery, cancellationToken);
        if (suggestion == null)
            return RabbitConsumptionResult.Reject;

        suggestion.VoteSession!.VoteMessageId = message.MessageId;
        await DbContext.SaveChangesAsync(cancellationToken);

        return RabbitConsumptionResult.Success;
    }
}
