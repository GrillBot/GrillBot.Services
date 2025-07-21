using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionApprovalChangeHandler(IServiceProvider serviceProvider) : EmoteSuggestionHandlerBase<EmoteSuggestionApprovalChangePayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        EmoteSuggestionApprovalChangePayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        ArgumentOutOfRangeException.ThrowIfZero(message.ApprovedByUserId);

        var suggestionQuery = DbContext.EmoteSuggestions.Where(o => o.Id == message.SuggestionId && o.VoteSession == null);
        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(suggestionQuery, cancellationToken);

        if (suggestion == null)
            return RabbitConsumptionResult.Reject;

        var guildQuery = DbContext.Guilds.Where(o => o.GuildId == suggestion.GuildId && o.SuggestionChannelId != 0);
        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery, cancellationToken);

        if (guild is null)
        {
            Logger.LogWarning("Rejecting EmoteSuggestionApprovalChange message, because guild with suggestion channel is not configured.");
            return RabbitConsumptionResult.Reject;
        }

        suggestion.ApprovedForVote = message.IsApprovedForVote;
        suggestion.ApprovalByUserId = message.ApprovedByUserId;
        suggestion.ApprovalSetAtUtc = DateTime.UtcNow;
        await ContextHelper.SaveChangesAsync(cancellationToken);

        var notificationMesasge = CreateAdminChannelNotification(suggestion, guild, suggestion.SuggestionMessageId);
        await Publisher.PublishAsync((DiscordEditMessagePayload)notificationMesasge, cancellationToken: cancellationToken);

        return RabbitConsumptionResult.Success;
    }
}
