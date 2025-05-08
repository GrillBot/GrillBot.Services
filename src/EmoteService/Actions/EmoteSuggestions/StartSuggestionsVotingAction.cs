using EmoteService.Core.Entity;
using EmoteService.Core.Entity.Suggestions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages.Components;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages.Embeds;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace EmoteService.Actions.EmoteSuggestions;

public class StartSuggestionsVotingAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext,
    IRabbitPublisher _rabbitPublisher
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);

        var guildQuery = DbContext.Guilds.AsNoTracking()
            .Where(o => o.GuildId == guildId && o.VoteChannelId != 0);
        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery);

        if (guild is null)
        {
            var validationErrors = new ModelStateDictionary();
            validationErrors.AddModelError("GuildId", "Guild not found or vote channel is not set.");

            return ApiResult.BadRequest(validationErrors);
        }

        var suggestionsQuery = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession)
            .Where(o => o.ApprovedForVote && o.VoteSession == null);

        var suggestions = await ContextHelper.ReadEntitiesAsync(suggestionsQuery);
        var voteMessages = new List<DiscordSendMessagePayload>();
        foreach (var suggestion in suggestions)
            voteMessages.Add(ProcessSuggestionToVote(suggestion, guild));

        await _rabbitPublisher.PublishAsync(voteMessages);
        await DbContext.SaveChangesAsync();

        return ApiResult.Ok(suggestions.Count);
    }

    private static DiscordSendMessagePayload ProcessSuggestionToVote(EmoteSuggestion suggestion, Core.Entity.Guild guild)
    {
        var startAt = DateTime.UtcNow;

        suggestion.VoteSession = new EmoteVoteSession
        {
            ExpectedVoteEndAtUtc = startAt.Add(guild.VoteTime),
            Id = suggestion.Id,
            KilledAtUtc = null,
            VoteStartedAtUtc = startAt
        };

        return CreateVoteMessage(suggestion, guild);
    }

    private static DiscordSendMessagePayload CreateVoteMessage(EmoteSuggestion suggestion, Core.Entity.Guild guild)
    {
        var image = suggestion.CreateImageFile();
        var embed = CreateVoteEmbed(suggestion, image);
        var components = CreateVoteComponents(suggestion);

        return new DiscordSendMessagePayload(
            guild.GuildId,
            guild.SuggestionChannelId,
            null,
            [image],
            "Emote",
            allowedMentions: null,
            flags: null,
            embed: embed,
            components: components
        );
    }

    private static DiscordMessageComponent CreateVoteComponents(EmoteSuggestion suggestion)
    {
        var components = new DiscordMessageComponent();
        var customIdPrefix = $"suggestion_vote:{suggestion.Id}";

        components.AddButton(new ButtonComponent(null, $"{customIdPrefix}:True", Discord.ButtonStyle.Success, "👍"));
        components.AddButton(new ButtonComponent(null, $"{customIdPrefix}:False", Discord.ButtonStyle.Danger, "👎"));
        return components;
    }

    private static DiscordMessageEmbed CreateVoteEmbed(EmoteSuggestion suggestion, DiscordMessageFile image)
    {
        return new DiscordMessageEmbed(
            url: null,
            title: "SuggestionModule/VoteEmbed/Title",
            description: null,
            author: null,
            color: Discord.Color.Gold.RawValue,
            footer: null,
            imageUrl: $"attachment://{image.Filename}",
            thumbnailUrl: null,
            fields: [
                new("SuggestionModule/SuggestionEmbed/EmoteNameTitle", suggestion.Name, false),
                new("SuggestionModule/VoteEmbed/ExpectedEnd", $"DateTime:{suggestion.VoteSession!.ExpectedVoteEndAtUtc}", false)
            ],
            timestamp: suggestion.VoteSession!.VoteStartedAtUtc,
            useCurrentTimestamp: false
        );
    }
}
