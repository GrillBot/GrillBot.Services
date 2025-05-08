using EmoteService.Core.Entity;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionMessageDeletedHandler(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<EmoteSuggestionMessageDeletedPayload, EmoteServiceContext>(serviceProvider)
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
        suggestion.ApprovalSetAtUtc = null;
        suggestion.ApprovalByUserId = null;

        if (suggestion.VoteSession is not null)
            suggestion.VoteSession.KilledAtUtc = DateTime.UtcNow;

        await ContextHelper.SaveChangesAsync();
        return RabbitConsumptionResult.Success;
    }
}
