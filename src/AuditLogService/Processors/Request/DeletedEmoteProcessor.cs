using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;

namespace AuditLogService.Processors.Request;

public class DeletedEmoteProcessor : RequestProcessorBase
{
    public DeletedEmoteProcessor(DiscordManager discordManager) : base(discordManager)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.EmoteDeleted)
            return;

        var auditLogs = await DiscordManager.GetAuditLogsAsync(entity.GuildId!.ToUlong(), actionType: ActionType.EmojiDeleted);
        var auditLogItem = auditLogs.FirstOrDefault(o => ((EmoteDeleteAuditLogData)o.Data).EmoteId == request.DeletedEmote!.EmoteId.ToUlong());
        if (auditLogItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var removedEmoteData = (EmoteDeleteAuditLogData)auditLogItem.Data;

        entity.UserId = auditLogItem.User.Id.ToString();
        entity.DiscordId = auditLogItem.Id.ToString();
        entity.DeletedEmote = new DeletedEmote
        {
            EmoteId = removedEmoteData.EmoteId.ToString(),
            EmoteName = removedEmoteData.Name
        };
    }
}
