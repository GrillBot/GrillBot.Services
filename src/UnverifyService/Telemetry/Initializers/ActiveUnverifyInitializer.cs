using GrillBot.Services.Common.Telemetry;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Enums;

namespace UnverifyService.Telemetry.Initializers;

public class ActiveUnverifyInitializer(
    IServiceProvider serviceProvider,
    UnverifyTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var contextHelper = CreateContextHelper<UnverifyContext>(provider);

        var query = contextHelper.DbContext.ActiveUnverifies
            .GroupBy(o => o.LogItem.OperationType)
            .Select(o => new
            {
                Operation = o.Key,
                Count = o.Count()
            });

        var data = await contextHelper.ReadToDictionaryAsync(query, o => o.Operation, o => o.Count, cancellationToken);

        _collector.ActiveUnverify.Set(data.TryGetValue(UnverifyOperationType.Unverify, out var unverifyCount) ? unverifyCount : 0);
        _collector.ActiveSelfUnverify.Set(data.TryGetValue(UnverifyOperationType.SelfUnverify, out var selfUnverifyCount) ? selfUnverifyCount : 0);
    }
}
