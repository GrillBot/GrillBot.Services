using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;

namespace AuditLogService.Processors.Request;

public class UnbanProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var auditLog = await FindAuditLogAsync(request);
        if (auditLog is null)
        {
            entity.CanCreate = false;
            return;
        }

        entity.UserId = auditLog.User.Id.ToString();
        entity.DiscordId = auditLog.Id.ToString();
        entity.CreatedAt = auditLog.CreatedAt.UtcDateTime;
        entity.Unban = new Unban { UserId = request.Unban!.UserId };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((UnbanAuditLogData)entry.Data).Target.Id == request.Unban!.UserId.ToUlong();
}
