using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
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

        var guild = await DiscordManager.GetGuildAsync(entity.GuildId!.ToUlong(), true);
        if (guild is null)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = (MemberRoleAuditLogData)auditLog.Data;
        if (logData.Target is null)
        {
            AddWarning($"AuditLogItem with ID {auditLog.Id} from guild {guild.Name} contains unknown target user.");
            entity.CanCreate = false;
            return;
        }

        entity.DiscordId = auditLog.Id.ToString();
        entity.CreatedAt = auditLog.CreatedAt.UtcDateTime;
        entity.UserId = auditLog.User.Id.ToString();
        entity.MemberRolesUpdated = CreateLogData(logData, guild);
    }

    private ISet<MemberRoleUpdated> CreateLogData(MemberRoleAuditLogData logData, IGuild guild)
    {
        var result = new HashSet<MemberRoleUpdated>();

        foreach (var item in logData.Roles)
        {
            var id = Guid.NewGuid();
            var role = guild.GetRole(item.RoleId);
            if (role is null)
                AddWarning($"MemberRoleUpdated data entity [Id: {id}] will contain default role color. Role not found.");

            result.Add(new MemberRoleUpdated
            {
                Id = id,
                IsAdded = item.Added,
                RoleColor = (role is null ? Color.Default : role.Color).ToString(),
                RoleId = item.RoleId.ToString(),
                RoleName = item.Name,
                UserId = logData.Target.Id.ToString()
            });
        }

        return result;
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => request.DiscordId.ToUlong() == entry.Id;
}
