using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Enums;
using UnverifyService.Models.Response.Logs.Detail;

namespace UnverifyService.Actions.Logs;

public partial class GetUnverifyLogDetailAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var id = GetParameter<Guid>(0);

        var logItemQuery = DbContext.LogItems
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new
            {
                o.Id,
                o.LogNumber,
                Parent = o.ParentLogItem == null ? null : new
                {
                    Id = o.ParentLogItemId!.Value,
                    o.ParentLogItem.LogNumber,
                    FromUserId = o.ParentLogItem.FromUserId.ToString(),
                    ToUserId = o.ParentLogItem.ToUserId.ToString(),
                    o.ParentLogItem.CreatedAt,
                    o.ParentLogItem.OperationType
                },
                o.OperationType,
                GuildId = o.GuildId.ToString(),
                FromUserId = o.FromUserId.ToString(),
                ToUserId = o.ToUserId.ToString(),
                o.CreatedAt
            });

        var logItem = await ContextHelper.ReadFirstOrDefaultEntityAsync(logItemQuery, CancellationToken);
        if (logItem is null)
            return ApiResult.NotFound();

        object? data = logItem.OperationType switch
        {
            UnverifyOperationType.Unverify => await CreateUnverifyOperationDetailDataAsync(logItem.Id),
            UnverifyOperationType.SelfUnverify => await CreateSelfUnverifyOperationDetailDataAsync(logItem.Id),
            UnverifyOperationType.AutoRemove => await CreateAutoRemoveOperationDetailDataAsync(logItem.Id),
            UnverifyOperationType.Update => await CreateUpdateOperationDetailDataAsync(logItem.Id),
            UnverifyOperationType.Recovery => await CreateRecoveryOperationDetailDataAsync(logItem.Id),
            _ => null
        };

        return ApiResult.Ok(new UnverifyLogDetail(
            logItem.Id,
            logItem.LogNumber,
            logItem.Parent is null ? null : new UnverifyLogSimpleDetail(
                logItem.Parent.Id,
                logItem.Parent.LogNumber,
                logItem.Parent.FromUserId,
                logItem.Parent.ToUserId,
                logItem.Parent.CreatedAt,
                logItem.Parent.OperationType
            ),
            logItem.OperationType,
            logItem.GuildId,
            logItem.FromUserId,
            logItem.ToUserId,
            logItem.CreatedAt,
            data
        ));
    }
}
