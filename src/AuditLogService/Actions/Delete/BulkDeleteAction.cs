using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Delete;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Delete;

public class BulkDeleteAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public BulkDeleteAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var ids = (List<Guid>)Parameters[0]!;
        var response = new BulkDeleteResponse();

        var logItems = await ReadLogItemsAsync(ids);
        foreach (var id in ids)
        {
            var responseItem = new DeleteItemResponse();
            response.Items.Add(responseItem);

            if (!logItems.TryGetValue(id, out var logItem))
                continue;

            responseItem.Exists = true;
            responseItem.FilesToDelete = logItem.Files.Select(o => o.Filename).Distinct().ToList();

            logItem.IsDeleted = true;
            logItem.IsPendingProcess = true;
        }

        await Context.SaveChangesAsync();
        return ApiResult.Ok(response);
    }

    private async Task<Dictionary<Guid, LogItem>> ReadLogItemsAsync(List<Guid> ids)
    {
        var items = await Context.LogItems
            .Include(o => o.Files)
            .Where(o => ids.Contains(o.Id))
            .ToListAsync();

        return items.ToDictionary(o => o.Id, o => o);
    }
}
