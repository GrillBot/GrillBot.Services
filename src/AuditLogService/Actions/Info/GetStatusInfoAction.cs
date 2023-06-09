﻿using AuditLogService.Core.Entity;
using AuditLogService.Core.Options;
using AuditLogService.Models.Response.Info;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuditLogService.Actions.Info;

public class GetStatusInfoAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }
    private AppOptions AppOptions { get; }

    public GetStatusInfoAction(AuditLogServiceContext context, IOptions<AppOptions> options)
    {
        Context = context;
        AppOptions = options.Value;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var expirationDate = DateTime.UtcNow.AddMonths(-AppOptions.ExpirationMonths);

        var result = new StatusInfo
        {
            ItemsToArchive = await Context.LogItems.AsNoTracking()
                .CountAsync(o => o.CreatedAt <= expirationDate)
        };

        return ApiResult.FromSuccess(result);
    }
}
