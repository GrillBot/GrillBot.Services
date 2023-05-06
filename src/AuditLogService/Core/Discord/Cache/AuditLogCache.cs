using Discord;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Core.Discord.Cache;

public sealed class AuditLogCache : IDisposable
{
    private static readonly object Locker = new();
    private readonly Dictionary<ulong, Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>>> _cachedEntries = new();

    private ICounterManager CounterManager { get; }

    public AuditLogCache(ICounterManager counterManager)
    {
        CounterManager = counterManager;
    }

    public IEnumerable<IAuditLogEntry>? GetAuditLogs(ulong guildId, ActionType type)
    {
        using (CounterManager.Create("Discord.Cache.AuditLog"))
        {
            lock (Locker)
            {
                return _cachedEntries.TryGetValue(guildId, out var guildLogs) && guildLogs.TryGetValue(type, out var logs) ? logs : null;
            }
        }
    }

    public void StoreLogs(ulong guildId, ActionType type, IReadOnlyCollection<IAuditLogEntry> logs)
    {
        using (CounterManager.Create("Discord.Cache.AuditLog"))
        {
            lock (Locker)
            {
                if (!_cachedEntries.ContainsKey(guildId))
                    _cachedEntries.Add(guildId, new Dictionary<ActionType, IReadOnlyCollection<IAuditLogEntry>>());

                if (!_cachedEntries[guildId].ContainsKey(type))
                    _cachedEntries[guildId].Add(type, logs);
                else
                    _cachedEntries[guildId][type] = logs;
            }
        }
    }

    public void Dispose()
    {
        lock (Locker)
        {
            foreach (var guild in _cachedEntries)
                guild.Value.Clear();

            _cachedEntries.Clear();
        }
    }
}
