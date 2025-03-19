using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord.Rest;

namespace AuditLogService.Processors.Request;

public class OverwriteDeletedProcessor(IServiceProvider serviceProvider) : OverwriteProcessorBase(serviceProvider)
{
    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var logItem = await FindAuditLogAsync(request);
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = (OverwriteDeleteAuditLogData)logItem.Data;
        var overwriteInfo = CreateOverwriteInfo(logData.Overwrite);

        entity.DiscordId = logItem.Id.ToString();
        entity.UserId = logItem.User.Id.ToString();
        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.OverwriteDeleted = new OverwriteDeleted
        {
            OverwriteInfo = overwriteInfo,
            OverwriteInfoId = overwriteInfo.Id
        };
    }
}
