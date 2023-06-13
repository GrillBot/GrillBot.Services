using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;
using Discord.Rest;

namespace AuditLogService.Processors.Request;

public class OverwriteCreatedProcessor : OverwriteProcessorBase
{
    public OverwriteCreatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var logItem = await FindAuditLogAsync(request);
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = (OverwriteCreateAuditLogData)logItem.Data;
        var overwriteInfo = CreateOverwriteInfo(logData.Overwrite);

        entity.DiscordId = logItem.Id.ToString();
        entity.UserId = logItem.User.Id.ToString();
        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.OverwriteCreated = new OverwriteCreated
        {
            OverwriteInfo = overwriteInfo,
            OverwriteInfoId = overwriteInfo.Id
        };
    }
}
