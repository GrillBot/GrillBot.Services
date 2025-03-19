using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Cache.Abstraction;

namespace GrillBot.Services.Common.Cache;

public class AuditLogCache(
    ICounterManager counterManager
) : ScopedCache<ulong, Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>>>(counterManager)
{
    public IEnumerable<IAuditLogEntry>? GetAuditLogs(ulong guildId, ActionType actionType)
    {
        if (!TryRead(guildId, out var guildLogs))
            return null;

        using (CreateCounterItem("GetAuditLogs"))
            return guildLogs.TryGetValue(actionType, out var logs) ? logs : null;
    }

    public void StoreLogs(ulong guildId, ActionType actionType, IReadOnlyCollection<IAuditLogEntry> logs)
    {
        using (CreateCounterItem("StoreLogs"))
        {
            if (!TryRead(guildId, out var guildLogs))
                guildLogs = [];

            guildLogs[actionType] = logs;
            Write(guildId, guildLogs);
        }
    }

    protected override void DisposeItem(Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>> data)
        => data.Clear();
}
