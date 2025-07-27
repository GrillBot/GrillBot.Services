using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Actions.Archivation;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Enums;
using UnverifyService.Models.Events;
using UnverifyService.Telemetry;

namespace UnverifyService.Handlers;

public class RecalculateMetricsHandler(
    IServiceProvider serviceProvider,
    UnverifyTelemetryCollector _collector,
    CreateArchivationDataAction _archivationAction
) : BaseEventHandlerWithDb<RecalculateMetricsMessage, UnverifyContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        RecalculateMetricsMessage message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        await RecalculateActiveUnverifiesAsync(cancellationToken);
        await RecalculateUnverifyLogsAsync(cancellationToken);

        return RabbitConsumptionResult.Success;
    }

    private async Task RecalculateActiveUnverifiesAsync(CancellationToken cancellationToken = default)
    {
        var query = DbContext.ActiveUnverifies.AsNoTracking()
            .GroupBy(o => o.LogItem.OperationType)
            .Select(o => new { o.Key, Count = o.Count() });

        var data = await ContextHelper.ReadToDictionaryAsync(query, o => o.Key, o => o.Count, cancellationToken);

        _collector.ActiveUnverify.Set(data.TryGetValue(UnverifyOperationType.Unverify, out var unverifyCount) ? unverifyCount : 0);
        _collector.ActiveSelfUnverify.Set(data.TryGetValue(UnverifyOperationType.SelfUnverify, out var selfUnverifyCount) ? selfUnverifyCount : 0);
    }

    private async Task RecalculateUnverifyLogsAsync(CancellationToken cancellationToken = default)
    {
        _archivationAction.SetCancellationToken(cancellationToken);

        _collector.ItemsToArchive.Set(await _archivationAction.CountAsync());
    }
}
