using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions;

public class DeleteItemAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public DeleteItemAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var id = (Guid)Parameters[0]!;
        var response = new DeleteItemResponse();

        var logItem = await ReadLogItemAsync(id);
        if (logItem is null)
            return new ApiResult(StatusCodes.Status404NotFound, response);

        response.FilesToDelete = logItem.Files.Select(o => o.Filename).Distinct().ToList();
        response.Exists = true;

        logItem.IsDeleted = true;
        logItem.IsPendingProcess = true;

        await Context.SaveChangesAsync();
        return ApiResult.FromSuccess(response);
    }

    private async Task<LogItem?> ReadLogItemAsync(Guid id)
    {
        return await Context.LogItems
            .Include(o => o.Files)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }
}
