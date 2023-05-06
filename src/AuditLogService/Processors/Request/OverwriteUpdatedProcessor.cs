using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using Discord;
using Discord.Rest;

namespace AuditLogService.Processors.Request;

public class OverwriteUpdatedProcessor : OverwriteProcessorBase
{
    public OverwriteUpdatedProcessor(DiscordManager discordManager, AuditLogServiceContext context) : base(discordManager, context)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.OverwriteUpdated)
            return;

        var logItem = await FindAuditLogAsync(entity.GuildId!, entity.ChannelId!, ActionType.OverwriteUpdated);
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
        entity.OverwriteUpdated = new OverwriteUpdated
        {
            After = after,
            Before = before,
            AfterId = after.Id,
            BeforeId = before.Id
        };
    }
}
