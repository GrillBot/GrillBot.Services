using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
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

        var logHeader = await Context.LogItems.AsNoTracking().FirstOrDefaultAsync(o => (o.Flags & LogItemFlag.Deleted) == 0 && o.Id == id);
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
        }

        return ApiResult.FromSuccess(result);
    }
}
