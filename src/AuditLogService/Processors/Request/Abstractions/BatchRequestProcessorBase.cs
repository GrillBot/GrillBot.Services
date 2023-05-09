using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using Discord;
using GrillBot.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Processors.Request.Abstractions;

public abstract class BatchRequestProcessorBase : RequestProcessorBase
{
    private AuditLogServiceContext Context { get; }

    protected BatchRequestProcessorBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Context = serviceProvider.GetRequiredService<AuditLogServiceContext>();
    }

    private async Task<HashSet<ulong>> GetIgnoredDiscordIdsAsync(LogRequest logRequest)
    {
        var timeLimit = DateTime.UtcNow.AddMonths(-2);

        var items = await Context.LogItems.AsNoTracking()
            .Where(o => o.DiscordId != null && o.Type == logRequest.Type && o.GuildId == logRequest.GuildId && o.ChannelId == logRequest.ChannelId && o.CreatedAt >= timeLimit)
            .Select(o => o.DiscordId!)
            .ToListAsync();

        return items
            .SelectMany(o => o.Split(','))
            .Select(o => o.Trim().ToUlong())
            .Distinct()
            .ToHashSet();
    }

    private static ActionType ConvertActionType(LogType type)
    {
        return type switch
        {
            LogType.OverwriteCreated => ActionType.OverwriteCreated,
            LogType.OverwriteDeleted => ActionType.OverwriteDeleted,
            LogType.OverwriteUpdated => ActionType.OverwriteUpdated,
            LogType.MemberRoleUpdated => ActionType.MemberRoleUpdated,
            _ => throw new ArgumentException($"Unsupported type ({type})", nameof(type))
        };
    }

    protected async Task<IAuditLogEntry?> FindAuditLogAsync(LogRequest logRequest)
    {
        var actionType = ConvertActionType(logRequest.Type);
        var ignoredLogIds = await GetIgnoredDiscordIdsAsync(logRequest);
        var auditLogs = await DiscordManager.GetAuditLogsAsync(logRequest.GuildId.ToUlong(), actionType: actionType);

        return auditLogs
            .FirstOrDefault(o => !ignoredLogIds.Contains(o.Id) && IsValidItem(o, logRequest));
    }

    protected async Task<List<IAuditLogEntry>> FindAuditLogsAsync(LogRequest logRequest)
    {
        var actionType = ConvertActionType(logRequest.Type);
        var ignoredLogIds = await GetIgnoredDiscordIdsAsync(logRequest);
        var auditLogs = await DiscordManager.GetAuditLogsAsync(logRequest.GuildId.ToUlong(), actionType: actionType);

        return auditLogs
            .Where(o => !ignoredLogIds.Contains(o.Id) && IsValidItem(o, logRequest))
            .ToList();
    }

    protected abstract bool IsValidItem(IAuditLogEntry entry, LogRequest request);
}
