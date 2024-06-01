using Discord;
using GrillBot.Core.Extensions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Services.Common.Discord;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class MemberWarningEventHandler : BaseMeasuresHandler<MemberWarningPayload>
{
    private readonly DiscordManager _discordManager;

    public MemberWarningEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher,
        DiscordManager discordManager) : base(loggerFactory, dbContext, counterManager, publisher)
    {
        _discordManager = discordManager;
    }

    protected override async Task HandleInternalAsync(MemberWarningPayload payload, Dictionary<string, string> headers)
    {
        var entity = new MemberWarningItem
        {
            CreatedAtUtc = payload.CreatedAtUtc.ToUniversalTime(),
            GuildId = payload.GuildId,
            ModeratorId = payload.ModeratorId,
            Reason = payload.Reason,
            UserId = payload.TargetUserId
        };

        await SaveEntityAsync(entity);

        if (payload.SendDmNotification)
            await SendNotificationToUserAsync(entity);
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
            new[]
            {
                new DiscordMessageEmbedField("Server", guild?.Name ?? "Neznámý server", true),
                new DiscordMessageEmbedField("Obsah varování", item.Reason, false)
            },
            null,
            true
        );

        var message = new DiscordMessagePayload(null, item.UserId, null, Enumerable.Empty<DiscordMessageFile>(), "UserMeasures", null, null, embed);
        await Publisher.PublishAsync(message, new());
    }
}
