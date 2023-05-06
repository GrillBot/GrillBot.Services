using System.Formats.Tar;
using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Processors.Request;

public abstract class OverwriteProcessorBase : RequestProcessorBase
{
    private AuditLogServiceContext Context { get; }

    protected OverwriteProcessorBase(DiscordManager discordManager, AuditLogServiceContext context) : base(discordManager)
    {
        Context = context;
    }

    private async Task<HashSet<ulong>> GetIgnoredDiscordIds(string guildId, string channelId)
    {
        var types = new[] { LogType.OverwriteCreated, LogType.OverwriteDeleted, LogType.OverwriteUpdated };
        var timeLimit = DateTime.UtcNow.AddMonths(-2);

        var items = await Context.LogItems.AsNoTracking()
            .Where(o => o.DiscordId != null && types.Contains(o.Type) && o.GuildId == guildId && o.ChannelId == channelId && o.CreatedAt >= timeLimit)
            .Select(o => o.DiscordId!)
            .ToListAsync();

        return items
            .SelectMany(o => o.Split(','))
            .Select(o => o.Trim().ToUlong())
            .Distinct()
            .ToHashSet();
    }

    protected async Task<IAuditLogEntry?> FindAuditLogAsync(string guildId, string channelId, ActionType actionType)
    {
        var ignoredLogIds = await GetIgnoredDiscordIds(guildId, channelId);
        var auditLogs = await DiscordManager.GetAuditLogsAsync(guildId.ToUlong(), actionType: actionType);

        return auditLogs
            .FirstOrDefault(o => IsValidItem(o, ignoredLogIds, channelId));
    }

    private static bool IsValidItem(IAuditLogEntry entry, IReadOnlySet<ulong> ignoredIds, string channelId)
    {
        if (ignoredIds.Contains(entry.Id)) return false;

        return entry.Action switch
        {
            ActionType.OverwriteCreated => ((OverwriteCreateAuditLogData)entry.Data).ChannelId == channelId.ToUlong(),
            ActionType.OverwriteDeleted => ((OverwriteDeleteAuditLogData)entry.Data).ChannelId == channelId.ToUlong(),
            ActionType.OverwriteUpdated => ((OverwriteUpdateAuditLogData)entry.Data).ChannelId == channelId.ToUlong(),
            _ => false
        };
    }

    protected static OverwriteInfo CreateOverwriteInfo(Overwrite overwrite)
    {
        return new OverwriteInfo
        {
            Id = Guid.NewGuid(),
            Target = overwrite.TargetType,
            AllowValue = overwrite.Permissions.AllowValue.ToString(),
            TargetId = overwrite.TargetId.ToString(),
            DenyValue = overwrite.Permissions.DenyValue.ToString()
        };
    }
}
