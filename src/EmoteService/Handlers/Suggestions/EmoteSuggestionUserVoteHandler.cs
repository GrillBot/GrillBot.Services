using EmoteService.Core.Entity.Suggestions;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionUserVoteHandler(
    IServiceProvider serviceProvider
) : EmoteSuggestionHandlerBase<EmoteSuggestionUserVotePayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteSuggestionUserVotePayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        ArgumentOutOfRangeException.ThrowIfZero(message.UserId);

        var suggestionQuery = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession).ThenInclude(o => o!.UserVotes)
            .Where(o =>
                o.Id == message.SuggestionId &&
                o.VoteSession != null &&
                o.VoteSession.KilledAtUtc == null &&
                !o.VoteSession.IsClosed
            );

        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(suggestionQuery);
        if (suggestion == null)
            return RabbitConsumptionResult.Reject;

        var guildQuery = DbContext.Guilds.Where(o => o.GuildId == suggestion.GuildId && o.SuggestionChannelId != 0);
        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery);
        if (guild is null)
            return RabbitConsumptionResult.Reject;

        var userVote = suggestion.VoteSession!.UserVotes.FirstOrDefault(o => o.UserId == message.UserId);
        if (userVote is null)
        {
            userVote = new EmoteUserVote
            {
                UserId = message.UserId
            };

            suggestion.VoteSession.UserVotes.Add(userVote);
        }

        userVote.IsApproved = message.IsApproved;
        userVote.UpdatedAtUtc = DateTime.UtcNow;
        await ContextHelper.SaveChagesAsync();

        var notificationMessage = CreateAdminChannelNotification(suggestion, guild, suggestion.SuggestionMessageId);
        await Publisher.PublishAsync((DiscordEditMessagePayload)notificationMessage);

        return RabbitConsumptionResult.Success;
    }
}
