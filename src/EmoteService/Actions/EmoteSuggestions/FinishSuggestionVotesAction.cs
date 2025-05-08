using EmoteService.Core.Entity;
using EmoteService.Core.Entity.Suggestions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages.Embeds;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.EmoteSuggestions;

public class FinishSuggestionVotesAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext,
    IRabbitPublisher _rabbitPublisher
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var suggestionsQuery = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession).ThenInclude(o => o!.UserVotes)
            .Where(o =>
                o.VoteSession != null &&
                o.VoteSession!.KilledAtUtc == null &&
                !o.VoteSession.IsClosed &&
                o.VoteSession.ExpectedVoteEndAtUtc <= DateTime.UtcNow
            );

        var suggestions = await ContextHelper.ReadEntitiesAsync(suggestionsQuery);
        if (suggestions.Count == 0)
            return ApiResult.Ok(0);

        var messages = new List<DiscordSendMessagePayload>();
        foreach (var group in suggestions.GroupBy(o => o.GuildId))
        {
            var message = await FinalizeAndCreateReport(group.Key, group);
            if (message is not null)
                messages.Add(message);
        }

        if (messages.Count > 0)
            await _rabbitPublisher.PublishAsync(messages);
        await ContextHelper.SaveChagesAsync();

        return ApiResult.Ok(suggestions.Count);
    }

    private async Task<DiscordSendMessagePayload?> FinalizeAndCreateReport(ulong guildId, IEnumerable<EmoteSuggestion> suggestions)
    {
        foreach (var suggestion in suggestions)
        {
            suggestion.VoteSession!.IsClosed = true;
        }

        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync<Core.Entity.Guild>(o => o.GuildId == guildId && o.SuggestionChannelId != 0);
        if (guild is null)
            return null;

        var reportEmbed = new DiscordMessageEmbed(
            url: null,
            title: "SuggestionModule/VoteReport/Title",
            description: null,
            author: new DiscordMessageEmbedAuthor
            {
                IconUrl = "Bot.AvatarUrl",
                Name = "Bot.DisplayName"
            },
            color: Discord.Color.Blue.RawValue,
            footer: null,
            imageUrl: null,
            thumbnailUrl: null,
            fields: suggestions.Select(o => new DiscordMessageEmbedField(
                name: o.Name,
                value: $"**ID**: {o.Id}\n👍: {o.VoteSession!.UpVotes()}\n👎: {o.VoteSession.DownVotes()}",
                isInline: false
            )),
            timestamp: null,
            useCurrentTimestamp: true
        );

        var message = new DiscordSendMessagePayload(
            guild.GuildId,
            guild.SuggestionChannelId,
            null,
            [],
            "Emote",
            embed: reportEmbed
        );

        message.WithLocalization(locale: "cs-CZ");
        return message;
    }
}
