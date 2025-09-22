using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using ThreadInfo = AuditLogService.Core.Entity.ThreadInfo;

namespace AuditLogService.Processors.Request;

public class ThreadDeletedProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var auditLog = await FindAuditLogAsync(request);
        if (auditLog is null)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = (ThreadDeleteAuditLogData)auditLog.Data;
        var threadInfo = new ThreadInfo
        {
            Id = Guid.NewGuid(),
            Tags = request.ThreadInfo?.Tags,
            Type = logData.ThreadType,
            ArchiveDuration = logData.AutoArchiveDuration,
            IsArchived = logData.IsArchived,
            IsLocked = logData.IsLocked,
            SlowMode = logData.SlowModeInterval,
            ThreadName = logData.ThreadName
        };

        if (threadInfo.Tags?.Count == 0)
            threadInfo.Tags = null;

        entity.CreatedAt = auditLog.CreatedAt.UtcDateTime;
        entity.DiscordId = auditLog.Id.ToString();
        entity.UserId = auditLog.User.Id.ToString();
        entity.ThreadDeleted = new ThreadDeleted
        {
            ThreadInfo = threadInfo,
            ThreadInfoId = threadInfo.Id
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((ThreadDeleteAuditLogData)entry.Data).ThreadId == request.ChannelId!.ToUlong();
}
