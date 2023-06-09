using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using GuildInfo = AuditLogService.Core.Entity.GuildInfo;

namespace AuditLogService.Processors.Request;

public class GuildUpdatedProcessor : RequestProcessorBase
{
    public GuildUpdatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var logItems = await FindAuditLogsAsync(request);
        var logItem = logItems.MaxBy(o => o.CreatedAt.LocalDateTime);
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var before = CreateGuildInfo(request.GuildUpdated!.Before!);
        var after = CreateGuildInfo(request.GuildUpdated!.After!);

        entity.UserId = logItem.User.Id.ToString();
        entity.DiscordId = logItem.Id.ToString();
        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.GuildUpdated = new GuildUpdated
        {
            Before = before,
            After = after,
            AfterId = after.Id,
            BeforeId = before.Id
        };
    }

    private static GuildInfo CreateGuildInfo(GuildInfoRequest request)
    {
        return new GuildInfo
        {
            Description = request.Description,
            Id = Guid.NewGuid(),
            Features = request.Features,
            Name = request.Name,
            AfkTimeout = request.AfkTimeout,
            BannerId = request.BannerId,
            IconData = request.IconData,
            IconId = request.IconId,
            MfaLevel = request.MfaLevel,
            NsfwLevel = request.NsfwLevel,
            OwnerId = request.OwnerId,
            PremiumTier = request.PremiumTier,
            SplashId = request.SplashId,
            VanityUrl = request.VanityUrl,
            VerificationLevel = request.VerificationLevel,
            AfkChannelId = request.AfkChannelId,
            DefaultMessageNotifications = request.DefaultMessageNotifications,
            DiscoverySplashId = request.DiscoverySplashId,
            ExplicitContentFilter = request.ExplicitContentFilter,
            RulesChannelId = request.RulesChannelId,
            SystemChannelFlags = request.SystemChannelFlags,
            SystemChannelId = request.SystemChannelId,
            VoiceRegionId = request.VoiceRegionId,
            PublicUpdatesChannelId = request.PublicUpdatesChannelId
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => entry.CreatedAt.UtcDateTime >= DateTime.UtcNow.AddMinutes(-5);
}
