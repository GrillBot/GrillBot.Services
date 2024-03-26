using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Detail;

public partial class ReadDetailAction : ApiAction<AuditLogServiceContext>
{
    public ReadDetailAction(AuditLogServiceContext context, ICounterManager counterManager) : base(counterManager, context)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var id = (Guid)Parameters[0]!;

        var logHeader = await ReadHeaderAsync(id);
        if (logHeader is null)
            return new ApiResult(StatusCodes.Status404NotFound);

        var result = new Models.Response.Detail.Detail
        {
            Type = logHeader.Type
        };

        switch (logHeader.Type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                result.Data = await CreateMessageDetailAsync(logHeader);
                break;
            case LogType.ChannelUpdated:
                result.Data = await CreateChannelUpdatedDetailAsync(logHeader);
                break;
            case LogType.OverwriteUpdated:
                result.Data = await CreateOverwriteUpdatedDetailAsync(logHeader);
                break;
            case LogType.MemberUpdated:
                result.Data = await CreateMemberUpdatedDetailAsync(logHeader);
                break;
            case LogType.GuildUpdated:
                result.Data = await CreateGuildUpdatedDetailAsync(logHeader);
                break;
            case LogType.MessageDeleted:
                result.Data = await CreateMessageDeletedDetailAsync(logHeader);
                break;
            case LogType.InteractionCommand:
                result.Data = await CreateInteractionCommandDetailAsync(logHeader);
                break;
            case LogType.ThreadDeleted:
                result.Data = await CreateThreadDeletedDetailAsync(logHeader);
                break;
            case LogType.JobCompleted:
                result.Data = await CreateJobExecutionDetailAsync(logHeader);
                break;
            case LogType.Api:
                result.Data = await CreateApiRequestDetailAsync(logHeader);
                break;
            case LogType.ThreadUpdated:
                result.Data = await CreateThreaduUpdatedDetailAsync(logHeader);
                break;
            case LogType.RoleDeleted:
                result.Data = await CreateRoleDeletedDetailAsync(logHeader);
                break;
        }

        return ApiResult.Ok(result);
    }

    private async Task<LogItem?> ReadHeaderAsync(Guid id)
    {
        var query = DbContext.LogItems.Where(o => o.Id == id).AsNoTracking();
        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }
}
