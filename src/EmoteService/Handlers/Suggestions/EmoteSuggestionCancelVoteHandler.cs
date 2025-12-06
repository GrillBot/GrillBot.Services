using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Models.Events.Messages;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionCancelVoteHandler(IServiceProvider serviceProvider) : EmoteSuggestionHandlerBase<EmoteSuggestionCancelVotePayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        EmoteSuggestionCancelVotePayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        var suggestionQuery = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession)
            .Where(o =>
                o.Id == message.SuggestionId &&
                o.VoteSession != null &&
                o.VoteSession.KilledAtUtc == null &&
                o.VoteSession.ExpectedVoteEndAtUtc >= DateTime.UtcNow &&
                !o.VoteSession.IsClosed
            );

        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(suggestionQuery, cancellationToken);
        if (suggestion == null)
            return RabbitConsumptionResult.Reject;

        var guildQuery = DbContext.Guilds.Where(o => o.GuildId == suggestion.GuildId);
        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery, cancellationToken);
        if (guild == null)
            return RabbitConsumptionResult.Reject;

        suggestion.VoteSession!.KilledAtUtc = DateTime.UtcNow;

        await ContextHelper.SaveChangesAsync(cancellationToken);
        var notificationMessage = CreateAdminChannelNotification(suggestion, guild, suggestion.SuggestionMessageId);
        await Publisher.PublishAsync((DiscordEditMessagePayload)notificationMessage, cancellationToken: cancellationToken);

        return RabbitConsumptionResult.Success;
    }
}
