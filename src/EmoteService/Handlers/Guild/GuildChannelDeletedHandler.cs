using EmoteService.Core.Entity;
using EmoteService.Models.Events.Guild;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace EmoteService.Handlers.Guild;

public class GuildChannelDeletedHandler(IServiceProvider serviceProvider) : BaseEventHandlerWithDb<GuildChannelDeletedPayload, EmoteServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        GuildChannelDeletedPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentOutOfRangeException.ThrowIfZero(message.GuildId);
        ArgumentOutOfRangeException.ThrowIfZero(message.ChannelId);

        var guildQuery = DbContext.Guilds
            .Where(o => o.GuildId == message.GuildId && (o.SuggestionChannelId == message.ChannelId || o.VoteChannelId == message.ChannelId));

        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery, cancellationToken);
        if (guild is null)
            return RabbitConsumptionResult.Success;

        // Clear configuration if channel is private channel for suggestions.
        if (guild.SuggestionChannelId == message.ChannelId)
            guild.SuggestionChannelId = 0;

        // Clear configuration if channel is channel for votes and kill active vote sessions.
        if (guild.VoteChannelId == message.ChannelId)
        {
            guild.VoteChannelId = 0;

            // Kill all active vote sessions for this guild.
            var voteSessionsQuery = DbContext.EmoteVoteSessions
                .Where(o => o.KilledAtUtc == null && o.ExpectedVoteEndAtUtc > DateTime.UtcNow && o.Suggestion.GuildId == message.GuildId);

            var votes = await ContextHelper.ReadEntitiesAsync(voteSessionsQuery, cancellationToken);
            foreach (var vote in votes)
                vote.KilledAtUtc = DateTime.UtcNow;
        }

        await ContextHelper.SaveChangesAsync(cancellationToken);
        return RabbitConsumptionResult.Success;
    }
}
