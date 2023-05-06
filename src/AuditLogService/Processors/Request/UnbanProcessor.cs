using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;

namespace AuditLogService.Processors.Request;

public class UnbanProcessor : RequestProcessorBase
{
    public UnbanProcessor(DiscordManager discordManager) : base(discordManager)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.Unban)
            return;

        var auditLogs = await DiscordManager.GetAuditLogsAsync(entity.GuildId!.ToUlong(), actionType: ActionType.Unban);
        var logItem = auditLogs.FirstOrDefault(o => ((UnbanAuditLogData)o.Data).Target.Id == request.Unban!.UserId.ToUlong());
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        entity.UserId = logItem.User.Id.ToString();
        entity.DiscordId = logItem.Id.ToString();
        entity.Unban = new Unban { UserId = request.Unban!.UserId };
    }
}
