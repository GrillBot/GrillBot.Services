using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response.Detail;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Detail;

public partial class ReadDetailAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; set; }

    public ReadDetailAction(AuditLogServiceContext context)
    {
        Context = context;
    }


    public override async Task<ApiResult> ProcessAsync()
    {
        var id = (Guid)Parameters[0]!;

        var logHeader = await Context.LogItems.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        if (logHeader is null)
            return new ApiResult(StatusCodes.Status404NotFound);

        object? detailDto = null;
        switch (logHeader.Type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                detailDto = await CreateMessageDetailAsync(logHeader);
                break;
            case LogType.ChannelUpdated:
                detailDto = await CreateChannelUpdatedDetailAsync(logHeader);
                break;
            case LogType.OverwriteUpdated:
                detailDto = await CreateOverwriteUpdatedDetailAsync(logHeader);
                break;
            case LogType.MemberUpdated:
                detailDto = await CreateMemberUpdatedDetailAsync(logHeader);
                break;
            case LogType.GuildUpdated:
                detailDto = await CreateGuildUpdatedDetailAsync(logHeader);
                break;
            case LogType.MessageDeleted:
                detailDto = await CreateMessageDeletedDetailAsync(logHeader);
                break;
            case LogType.InteractionCommand:
                detailDto = await CreateInteractionCommandDetailAsync(logHeader);
                break;
            case LogType.ThreadDeleted:
                detailDto = await CreateThreadDeletedDetailAsync(logHeader);
                break;
            case LogType.JobCompleted:
                detailDto = await CreateJobExecutionDetailAsync(logHeader);
                break;
            case LogType.Api:
                detailDto = await CreateApiRequestDetailAsync(logHeader);
                break;
            case LogType.ThreadUpdated:
                detailDto = await CreateThreaduUpdatedDetailAsync(logHeader);
                break;
        }

        return detailDto is null ? new ApiResult(StatusCodes.Status404NotFound) : new ApiResult(StatusCodes.Status200OK, detailDto);
    }
}
