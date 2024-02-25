using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
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

        return items.Select(o => o.ToUlong()).Distinct().ToHashSet();
    }

    protected async Task<IAuditLogEntry?> FindAuditLogAsync(LogRequest logRequest)
    {
        var ignoredLogIds = await GetIgnoredDiscordIdsAsync(logRequest);
        var auditLogs = await base.FindAuditLogsAsync(logRequest);

        return auditLogs.Find(o => !ignoredLogIds.Contains(o.Id));
    }

    protected async Task<List<IAuditLogEntry>> FindAuditLogsAsync(LogRequest logRequest)
    {
        var ignoredLogIds = await GetIgnoredDiscordIdsAsync(logRequest);
        var auditLogs = await base.FindAuditLogsAsync(logRequest);

        return auditLogs.FindAll(o => !ignoredLogIds.Contains(o.Id));
    }
}
