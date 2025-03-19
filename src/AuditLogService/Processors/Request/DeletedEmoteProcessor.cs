using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;

namespace AuditLogService.Processors.Request;

public class DeletedEmoteProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var logItem = await FindAuditLogAsync(request);
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var removedEmoteData = (EmoteDeleteAuditLogData)logItem.Data;

        entity.UserId = logItem.User.Id.ToString();
        entity.DiscordId = logItem.Id.ToString();
        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.DeletedEmote = new DeletedEmote
        {
            EmoteId = removedEmoteData.EmoteId.ToString(),
            EmoteName = removedEmoteData.Name
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((EmoteDeleteAuditLogData)entry.Data).EmoteId == Emote.Parse(request.DeletedEmote!.EmoteId).Id;
}
