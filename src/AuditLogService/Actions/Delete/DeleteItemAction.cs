﻿using AuditLogService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;

namespace AuditLogService.Actions.Delete;

public class DeleteItemAction : ApiAction<AuditLogServiceContext>
{
    public DeleteItemAction(ICounterManager counterManager, AuditLogServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var id = GetParameter<Guid>(0);

        var query = DbContext.LogItems.Where(o => o.Id == id && !o.IsDeleted);
        var item = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        if (item is null)
            return ApiResult.NotFound();

        item.IsDeleted = true;
        await ContextHelper.SaveChagesAsync();

        return ApiResult.Ok();
    }
}