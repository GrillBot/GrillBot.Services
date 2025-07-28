using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Models.Events;

namespace UnverifyService.Handlers;

public class LogBulkDeleteHandler(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<LogBulkDeleteMessage, UnverifyContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        LogBulkDeleteMessage message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        if (message.Ids.Count == 0)
            return RabbitConsumptionResult.Success;

        var logItems = await ReadLogItemsAsync(message.Ids, cancellationToken);
        if (logItems.Count == 0)
            return RabbitConsumptionResult.Success;

        DbContext.RemoveRange(logItems);
        await ContextHelper.SaveChangesAsync(cancellationToken);
        return RabbitConsumptionResult.Success;
    }

    private async Task<List<UnverifyLogItem>> ReadLogItemsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var result = new List<UnverifyLogItem>();

        var baseQuery = DbContext.LogItems
            .Include(o => o.ChildLogItems)
            .Include(o => o.RemoveOperation!).ThenInclude(o => o.Channels)
            .Include(o => o.RemoveOperation!).ThenInclude(o => o.Roles)
            .Include(o => o.SetOperation!).ThenInclude(o => o.Channels)
            .Include(o => o.SetOperation!).ThenInclude(o => o.Roles)
            .Include(o => o.UpdateOperation)
            .Where(o => o.ActiveUnverify == null)
            .AsSplitQuery();

        foreach (var chunk in ids.Distinct().Chunk(100))
        {
            var query = baseQuery.Where(o => chunk.Contains(o.Id));
            result.AddRange(await ContextHelper.ReadEntitiesAsync(query, cancellationToken));
        }

        return result;
    }
}
