using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;

namespace AuditLogService.Processors.Request;

public class RoleDeletedProcessor : RequestProcessorBase
{
    public RoleDeletedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var auditLog = await FindAuditLogAsync(request);
        if (auditLog is null)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = (RoleDeleteAuditLogData)auditLog.Data;
        var roleInfo = new RoleInfo
        {
            Color = logData.Properties.Color.ToString(),
            IconId = logData.Properties.IconId,
            Id = Guid.NewGuid(),
            IsHoisted = logData.Properties.Hoist ?? false,
            IsMentionable = logData.Properties.Mentionable ?? false,
            Name = logData.Properties.Name,
            RoleId = logData.RoleId.ToString()
        };

        if (logData.Properties.Permissions is not null)
            roleInfo.Permissions = logData.Properties.Permissions.Value.ToList().ConvertAll(o => o.ToString());

        entity.UserId = auditLog.User.Id.ToString();
        entity.DiscordId = auditLog.Id.ToString();
        entity.CreatedAt = auditLog.CreatedAt.UtcDateTime;
        entity.RoleDeleted = new RoleDeleted
        {
            RoleInfo = roleInfo,
            RoleInfoId = roleInfo.Id
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((RoleDeleteAuditLogData)entry.Data).RoleId.ToString() == request.RoleDeleted!.RoleId;
}
