using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using MemberRoleUpdated = AuditLogService.Core.Entity.MemberRoleUpdated;

namespace AuditLogService.Processors.Request;

public class MemberRoleUpdatedProcessor : BatchRequestProcessorBase
{
    public MemberRoleUpdatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
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

        var logData = (MemberRoleAuditLogData)auditLog.Data;
        var guild = await DiscordManager.GetGuildAsync(entity.GuildId!.ToUlong());

        entity.DiscordId = auditLog.Id.ToString();
        entity.CreatedAt = auditLog.CreatedAt.UtcDateTime;
        entity.UserId = auditLog.User.Id.ToString();
        entity.MemberRolesUpdated = logData.Roles.Select(o => new MemberRoleUpdated
        {
            Id = Guid.NewGuid(),
            UserId = logData.Target.Id.ToString(),
            IsAdded = o.Added,
            RoleColor = guild.GetRole(o.RoleId).Color.ToString(),
            RoleId = o.RoleId.ToString(),
            RoleName = o.Name
        }).ToHashSet();
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => request.DiscordId.ToUlong() == entry.Id;
}
