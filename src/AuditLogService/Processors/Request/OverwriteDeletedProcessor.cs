using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using Discord;
using Discord.Rest;

namespace AuditLogService.Processors.Request;

public class OverwriteDeletedProcessor : OverwriteProcessorBase
{
    public OverwriteDeletedProcessor(DiscordManager discordManager, AuditLogServiceContext context) : base(discordManager, context)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.OverwriteDeleted)
            return;

        var logItem = await FindAuditLogAsync(entity.GuildId!, entity.ChannelId!, ActionType.OverwriteDeleted);
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = (OverwriteDeleteAuditLogData)logItem.Data;
        var overwriteInfo = CreateOverwriteInfo(logData.Overwrite);

        entity.DiscordId = logItem.Id.ToString();
        entity.UserId = logItem.User.Id.ToString();
        entity.OverwriteDeleted = new OverwriteDeleted
        {
            OverwriteInfo = overwriteInfo,
            OverwriteInfoId = overwriteInfo.Id
        };
    }
}
