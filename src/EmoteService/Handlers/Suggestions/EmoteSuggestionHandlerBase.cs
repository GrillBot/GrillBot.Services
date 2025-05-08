using EmoteService.Core.Entity.Suggestions;
using EmoteService.Core.Entity;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages.Embeds;

namespace EmoteService.Handlers.Suggestions;

public abstract class EmoteSuggestionHandlerBase<TPayload>(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : BaseEventHandlerWithDb<TPayload, EmoteServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
    where TPayload : class, IRabbitMessage, new()
{
    protected DiscordMessagePayloadData CreateAdminChannelNotification(EmoteSuggestion suggestion, Core.Entity.Guild guild, ulong? suggestionMessageId)
    {
        var image = new DiscordMessageFile(
            $"{suggestion.Id}.{(suggestion.IsAnimated ? "gif" : "png")}",
            false,
            suggestion.Image
        );

        var embed = CreateNotificationEmbed(suggestion, image);

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
                embed: embed
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
                embed: embed
            );
        }

        message.WithLocalization(locale: "cs-CZ");
        message.ServiceData.Add("SuggestionId", suggestion.Id.ToString());
        // TODO Generate buttons for approval.

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
            color: Color.Blue.RawValue,
            footer: null,
            imageUrl: $"attachment://{image.Filename}",
            thumbnailUrl: null,
            fields: [
                new("SuggestionModule/SuggestionEmbed/EmoteNameTitle", suggestion.Name, false),
                new("SuggestionModule/SuggestionEmbed/EmoteReasonTitle", suggestion.ReasonForAdd, false),
                new($"SuggestionModule/SuggestionEmbed/ApprovedForVote/{suggestion.ApprovedForVote}", $"User.Mention:{suggestion.ApprovalByUserId}", true),
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

                embed.Color = (suggestion.VoteSession.IsCommunityApproved() ? Color.Green : Color.Red).RawValue;
            }
            else
            {
                embed.Description = "SuggestionModule/SuggestionEmbed/VoteRunning";
            }
        }

        return embed;
    }
}
