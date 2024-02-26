using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;

namespace AuditLogService.Processors.Request;

public class OverwriteUpdatedProcessor : OverwriteProcessorBase
{
    public OverwriteUpdatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
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

        var logData = (OverwriteUpdateAuditLogData)logItem.Data;
        var before = CreateOverwriteInfo(new Overwrite(logData.OverwriteTargetId, logData.OverwriteType, logData.OldPermissions));
        var after = CreateOverwriteInfo(new Overwrite(logData.OverwriteTargetId, logData.OverwriteType, logData.NewPermissions));

        entity.DiscordId = logItem.Id.ToString();
        entity.UserId = logItem.User.Id.ToString();
        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.OverwriteUpdated = new OverwriteUpdated
        {
            After = after,
            Before = before,
            AfterId = after.Id,
            BeforeId = before.Id
        };
    }
}
