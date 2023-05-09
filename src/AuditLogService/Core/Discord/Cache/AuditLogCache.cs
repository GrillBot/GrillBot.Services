using Discord;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Core.Discord.Cache;

public sealed class AuditLogCache : GuildCacheBase<Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>>>
{
    public AuditLogCache(ICounterManager counterManager) : base(counterManager)
    {
    }

    public IEnumerable<IAuditLogEntry>? GetAuditLogs(ulong guildId, ActionType type)
    {
        using (CounterManager.Create("Discord.Cache.AuditLog"))
        {
            lock (Locker)
            {
                return CachedData.TryGetValue(guildId, out var guildLogs) && guildLogs.TryGetValue(type, out var logs) ? logs : null;
            }
        }
    }

    public void StoreLogs(ulong guildId, ActionType type, IReadOnlyCollection<IAuditLogEntry> logs)
    {
        using (CounterManager.Create("Discord.Cache.AuditLog"))
        {
            lock (Locker)
            {
                if (!CachedData.ContainsKey(guildId))
                    CachedData.Add(guildId, new Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>>());

                if (!CachedData[guildId].ContainsKey(type))
                    CachedData[guildId].Add(type, logs);
                else
                    CachedData[guildId][type] = logs;
            }
        }
    }

    protected override void DisposeItem(Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>> data)
        => data.Clear();
}
