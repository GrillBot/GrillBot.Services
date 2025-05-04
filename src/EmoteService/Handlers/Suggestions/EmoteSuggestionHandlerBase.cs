using EmoteService.Core.Entity.Suggestions;
using EmoteService.Core.Entity;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Handlers.Suggestions;

public abstract class EmoteSuggestionHandlerBase<TPayload>(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : BaseEventHandlerWithDb<TPayload, EmoteServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
    where TPayload : class, IRabbitMessage, new()
{
    protected DiscordMessagePayload CreateAdminChannelNotification(EmoteSuggestion suggestion, Core.Entity.Guild guild)
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

        var message = new DiscordMessagePayload(
            guild.GuildId.ToString(),
            guild.SuggestionChannelId.ToString(),
            null,
            [image],
            "Emote",
            embed: embed
        );

        message.ServiceData.Add("UseLocalizedEmbeds", "true");
        message.ServiceData.Add("SuggestionId", suggestion.Id.ToString());
        message.ServiceData.Add("Language", "cs-CZ");

        return message;
    }
}
