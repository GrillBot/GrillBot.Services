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
    protected DiscordSendMessagePayload CreateAdminChannelNotification(EmoteSuggestion suggestion, Core.Entity.Guild guild)
    {
        var image = new DiscordMessageFile(
            $"{suggestion.Id}.{(suggestion.IsAnimated ? "gif" : "png")}",
            false,
            suggestion.Image
        );

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
                new("SuggestionModule/SuggestionEmbed/EmoteReasonTitle", suggestion.ReasonForAdd, false)
            ],
            timestamp: suggestion.SuggestedAtUtc,
            useCurrentTimestamp: false
        );

        var message = new DiscordSendMessagePayload(
            guild.GuildId,
            guild.SuggestionChannelId,
            null,
            [image],
            "Emote",
            embed: embed
        );

        message.WithLocalization(locale: "cs-CZ");
        message.ServiceData.Add("SuggestionId", suggestion.Id.ToString());

        return message;
    }
}
