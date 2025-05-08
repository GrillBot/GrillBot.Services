using EmoteService.Core.Entity;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;

namespace EmoteService.Handlers.Suggestions;

public class EmoteSuggestionApprovalChangeHandler(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : EmoteSuggestionHandlerBase<EmoteSuggestionApprovalChangePayload>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteSuggestionApprovalChangePayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        if (message.SuggestionId == Guid.Empty)
            return RabbitConsumptionResult.Reject;

        ArgumentOutOfRangeException.ThrowIfZero(message.ApprovedByUserId);

        var suggestionQuery = DbContext.EmoteSuggestions.Where(o => o.Id == message.SuggestionId && o.VoteSession == null);
        var suggestion = await ContextHelper.ReadFirstOrDefaultEntityAsync(suggestionQuery);

        if (suggestion == null)
            return RabbitConsumptionResult.Reject;

        var guildQuery = DbContext.Guilds.Where(o => o.GuildId == suggestion.GuildId && o.SuggestionChannelId != 0);
        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery);

        if (guild is null)
        {
            Logger.LogWarning("Rejecting EmoteSuggestionApprovalChange message, because guild with suggestion channel is not configured.");
            return RabbitConsumptionResult.Reject;
        }

        suggestion.ApprovedForVote = message.IsApprovedForVote;
        suggestion.ApprovalByUserId = message.ApprovedByUserId;
        suggestion.ApprovalSetAtUtc = DateTime.UtcNow;
        await ContextHelper.SaveChagesAsync();

        var notificationMesasge = CreateAdminChannelNotification(suggestion, guild, suggestion.SuggestionMessageId);
        await Publisher.PublishAsync((DiscordEditMessagePayload)notificationMesasge);

        return RabbitConsumptionResult.Success;
    }
}
