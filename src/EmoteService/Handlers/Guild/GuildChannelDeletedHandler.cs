using EmoteService.Core.Entity;
using EmoteService.Models.Events.Guild;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Handlers.Guild;

public class GuildChannelDeletedHandler(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : BaseEventHandlerWithDb<GuildChannelDeletedPayload, EmoteServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(GuildChannelDeletedPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        ArgumentOutOfRangeException.ThrowIfZero(message.GuildId);
        ArgumentOutOfRangeException.ThrowIfZero(message.ChannelId);

        var guild = await DbContext.Guilds
            .FirstOrDefaultAsync(o => o.GuildId == message.GuildId && (o.SuggestionChannelId == message.ChannelId || o.VoteChannelId == message.ChannelId));

        if (guild is null)
            return RabbitConsumptionResult.Success;

        // Clear configuration if channel is private channel for suggestions.
        if (guild.SuggestionChannelId == message.ChannelId)
            guild.SuggestionChannelId = 0;

        // Clear configuration if channel is channel for votes and kill active vote sessions.
        if (guild.VoteChannelId == message.ChannelId)
        {
            guild.VoteChannelId = 0;

            var votes = await DbContext.EmoteVoteSessions
                .Where(o => o.KilledAtUtc == null && o.ExpectedVoteEndAtUtc > DateTime.UtcNow)
                .ToListAsync();

            foreach (var vote in votes)
                vote.KilledAtUtc = DateTime.UtcNow;
        }

        await DbContext.SaveChangesAsync();
        return RabbitConsumptionResult.Success;
    }
}
