using GrillBot.Core.Database;
using GrillBot.Services.Common.Telemetry.Database;

namespace AuditLogService.Core.Providers;

public class StatisticsProvider(DatabaseTelemetryCollector _telemetry) : IStatisticsProvider
{
    public Task<Dictionary<string, long>> GetTableStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var values = _telemetry.TableCounts.Get();

        var result = values
            .Select(o => new KeyValuePair<string, long>(
                o.Tags.ToArray().First(x => x.Key == "table").Value!.ToString()!,
                o.Value
            ))
            .OrderBy(o => o.Key)
            .ToDictionary(o => o.Key, o => o.Value);

        return Task.FromResult(result);
    }
}
