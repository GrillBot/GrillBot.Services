using EmoteService.Core.Entity.Suggestions;
using EmoteService.Core.Entity;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages.Embeds;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages.Components;

namespace EmoteService.Handlers.Suggestions;

public abstract class EmoteSuggestionHandlerBase<TPayload>(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<TPayload, EmoteServiceContext>(serviceProvider)
    where TPayload : class, IRabbitMessage, new()
{
    protected static DiscordMessagePayloadData CreateAdminChannelNotification(EmoteSuggestion suggestion, Core.Entity.Guild guild, ulong? suggestionMessageId)
    {
        var image = suggestion.CreateImageFile();
        var embed = CreateNotificationEmbed(suggestion, image);
        var approvalButtons = CreateApprovalComponents(suggestion);

        DiscordMessagePayloadData message;
        if (suggestionMessageId is not null)
        {
            message = new DiscordEditMessagePayload(
                guild.GuildId,
                guild.SuggestionChannelId,
                suggestionMessageId.Value,
                null,
                [image],
                "Emote",
                embed: embed,
                components: approvalButtons
            );
        }
        else
        {
            message = new DiscordSendMessagePayload(
                guild.GuildId,
                guild.SuggestionChannelId,
                null,
                [image],
                "Emote",
                embed: embed,
                components: approvalButtons
            );
        }

        message.WithLocalization(locale: "cs-CZ");
        message.ServiceData.Add("SuggestionId", suggestion.Id.ToString());
        message.ServiceData.Add("MessageType", "SuggestionMessage");
        return message;
    }

    private static DiscordMessageEmbed CreateNotificationEmbed(EmoteSuggestion suggestion, DiscordMessageFile image)
    {
        var embed = new DiscordMessageEmbed(
            url: null,
            title: "SuggestionModule/SuggestionEmbed/Title",
            description: null,
            author: new DiscordMessageEmbedAuthor
            {
                IconUrl = $"User.AvatarUrl:{suggestion.FromUserId}",
                Name = $"User.DisplayName:{suggestion.FromUserId}"
            },
            color: Discord.Color.Blue.RawValue,
            footer: null,
            imageUrl: $"attachment://{image.Filename}",
            thumbnailUrl: null,
            fields: [
                new("SuggestionModule/SuggestionEmbed/EmoteNameTitle", suggestion.Name, false),
                new("SuggestionModule/SuggestionEmbed/EmoteReasonTitle", suggestion.ReasonForAdd, false),
                new($"SuggestionModule/SuggestionEmbed/ApprovedForVote/{suggestion.ApprovedForVote}/Title", $"User.Mention:{suggestion.ApprovalByUserId}", true),
                new($"SuggestionModule/SuggestionEmbed/ApprovedForVote/{suggestion.ApprovedForVote}/At", $"DateTime:{suggestion.ApprovalSetAtUtc}", true)
            ],
            timestamp: suggestion.SuggestedAtUtc,
            useCurrentTimestamp: false
        );

        if (suggestion.VoteSession is not null)
        {
            if (!suggestion.VoteSession.Running())
            {
                var voteEndAt = suggestion.VoteSession.KilledAtUtc ?? suggestion.VoteSession.ExpectedVoteEndAtUtc;

                embed.Title = suggestion.VoteSession.KilledAtUtc is not null ?
                    "SuggestionModule/SuggestionEmbed/VoteKilledTitle" :
                    "SuggestionModule/SuggestionEmbed/VoteFinishedTitle";

                embed.Fields.AddRange([
                    new("SuggestionModule/SuggestionEmbed/ApprovedVotes", suggestion.VoteSession.UpVotes().ToString(), true),
                    new("SuggestionModule/SuggestionEmbed/ApprovedVotes", suggestion.VoteSession.DownVotes().ToString(), true),
                    new("SuggestionModule/SuggestionEmbed/VoteStartedAt", $"DateTime:{suggestion.VoteSession.VoteStartedAtUtc}", true),
                    new("SuggestionModule/SuggestionEmbed/VoteEndAt", $"DateTime:{voteEndAt}", true)
                ]);

                embed.Color = (suggestion.VoteSession.IsCommunityApproved() ? Discord.Color.Green : Discord.Color.Red).RawValue;
            }
            else
            {
                embed.Description = "SuggestionModule/SuggestionEmbed/VoteRunning";
            }
        }

        return embed;
    }

    private static DiscordMessageComponent? CreateApprovalComponents(EmoteSuggestion suggestion)
    {
        if (suggestion.VoteSession is not null)
            return null;

        var component = new DiscordMessageComponent();
        var customIdPrefix = $"suggestion_approve_for_vote:{suggestion.Id}";

        component.AddButton(new ButtonComponent(null, $"{customIdPrefix}:True", Discord.ButtonStyle.Success, "👍"));
        component.AddButton(new ButtonComponent(null, $"{customIdPrefix}:False", Discord.ButtonStyle.Danger, "👎"));

        return component;
    }
}
