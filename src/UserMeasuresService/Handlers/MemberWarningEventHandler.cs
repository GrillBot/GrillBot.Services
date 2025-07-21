using Discord;
using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages.Embeds;
using GrillBot.Services.Common.Discord;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class MemberWarningEventHandler(
    IServiceProvider serviceProvider,
    DiscordManager _discordManager
) : BaseMeasuresHandler<MemberWarningPayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        MemberWarningPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var entity = new MemberWarningItem
        {
            CreatedAtUtc = message.CreatedAtUtc.ToUniversalTime(),
            GuildId = message.GuildId,
            ModeratorId = message.ModeratorId,
            Reason = message.Reason,
            UserId = message.TargetUserId
        };

        await SaveEntityAsync(entity);

        if (message.SendDmNotification)
            await SendNotificationToUserAsync(entity);
        return RabbitConsumptionResult.Success;
    }

    private async Task SendNotificationToUserAsync(MemberWarningItem item)
    {
        var guild = await _discordManager.GetGuildAsync(item.GuildId.ToUlong());

        var embed = new DiscordMessageEmbed(
            null,
            "Obdržel jsi upozornění",
            null,
            null,
            Color.Red.RawValue,
            null,
            null,
            null,
            [
                new DiscordMessageEmbedField("Server", guild?.Name ?? "Neznámý server", true),
                new DiscordMessageEmbedField("Obsah varování", item.Reason, false)
            ],
            null,
            true
        );

        var message = new DiscordSendMessagePayload(null, item.UserId.ToUlong(), null, [], "UserMeasures", null, null, embed);
        await Publisher.PublishAsync(message);
    }
}
