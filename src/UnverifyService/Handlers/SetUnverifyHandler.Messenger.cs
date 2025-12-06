using Discord;
using GrillBot.Core.Extensions;
using GrillBot.Core.Extensions.Discord;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Models;
using GrillBot.Models.Events.Messages;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Models;
using UnverifyService.Models.Events;

namespace UnverifyService.Handlers;

public partial class SetUnverifyHandler
{
    private Task SendSuccessSetDMAsync(UnverifySession session, CancellationToken cancellationToken = default)
    {
        var localizationArgs = new List<string>
        {
            session.TargetUser.Guild.Name,
            $"DateTime:{session.EndAtUtc:o}"
        };

        string localizationKey;
        if (session.IsSelfUnverify)
        {
            localizationKey = "Unverify/Message/PrivateUnverifyWithoutReason";
        }
        else
        {
            localizationKey = "Unverify/Message/PrivateUnverifyWithReason";
            localizationArgs.Add(session.Reason ?? "");
        }

        var discordMessage = new DiscordSendMessagePayload(
            guildId: null,
            channelId: session.TargetUser.Id,
            content: new(localizationKey, [.. localizationArgs]),
            attachments: [],
            serviceId: "Unverify"
        );

        discordMessage.WithLocalization(locale: session.TargetUserEntity?.Language ?? "en-US");
        return Publisher.PublishAsync(discordMessage, cancellationToken: cancellationToken);
    }

    private async Task SendUnverifyMessageToChannelAsync(UnverifySession session, SetUnverifyMessage message, ICurrentUserProvider currentUser, CancellationToken cancellationToken = default)
    {
        var localizationArgs = new List<string>
        {
            session.TargetUser.GetDisplayName(),
            $"DateTime:{session.EndAtUtc:o}"
        };

        string localizationKey;
        if (session.IsSelfUnverify)
        {
            localizationKey = "Unverify/Message/UnverifyToChannelWithoutReason";
        }
        else
        {
            localizationKey = "Unverify/Message/UnverifyToChannelWithReason";
            localizationArgs.Add(session.Reason ?? "");
        }

        var discordMessage = new DiscordSendMessagePayload(
            guildId: message.GuildId,
            channelId: message.ChannelId,
            content: new LocalizedMessageContent(localizationKey, [.. localizationArgs]),
            attachments: [],
            serviceId: "Unverify",
            reference: new(message.MessageId, message.ChannelId, message.GuildId, false, MessageReferenceType.Default)
        );

        var currentUserId = currentUser.Id.ToUlong();
        var localeQuery = DbContext.Users.AsNoTracking().Where(o => o.Id == currentUserId).Select(o => o.Language ?? "en-US");
        var currentUserLocale = (await ContextHelper.ReadFirstOrDefaultEntityAsync(localeQuery, cancellationToken)) ?? "en-US";

        discordMessage.WithLocalization(locale: currentUserLocale);
        await Publisher.PublishAsync(discordMessage, cancellationToken: cancellationToken);
    }
}
