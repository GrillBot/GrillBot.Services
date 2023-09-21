using AuditLogService.Cache.Abstraction;
using Discord;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Cache;

public class AuditLogCache : ScopeDisposableCache<ulong, Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>>>
{
    public AuditLogCache(ICounterManager counterManager) : base(counterManager)
    {
    }

    public IEnumerable<IAuditLogEntry>? GetAuditLogs(ulong guildId, ActionType actionType)
    {
        var guildLogs = Read(guildId);
        if (guildLogs is null)
            return null;

        using (CreateCounterItem("GetAuditLogs"))
        {
            return guildLogs.TryGetValue(actionType, out var logs) ? logs : null;
        }
    }

    public void StoreLogs(ulong guildId, ActionType actionType, IReadOnlyCollection<IAuditLogEntry> logs)
    {
        using (CreateCounterItem("StoreLogs"))
        {
            var guildLogs = Read(guildId);

            guildLogs ??= new Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>>();
            guildLogs[actionType] = logs;

            Write(guildId, guildLogs);
        }
    }

    protected override void DisposeItem(Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>> data)
        => data.Clear();
}
