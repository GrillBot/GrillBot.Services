using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using ThreadInfo = AuditLogService.Core.Entity.ThreadInfo;

namespace AuditLogService.Processors.Request;

public class ThreadUpdatedProcessor : RequestProcessorBase
{
    public ThreadUpdatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var logItems = await FindAuditLogsAsync(request);
        var logItem = logItems.MaxBy(o => o.CreatedAt.UtcDateTime);
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var before = CreateThreadInfo(request.ThreadUpdated!.Before!);
        var after = CreateThreadInfo(request.ThreadUpdated!.After!);

        entity.DiscordId = logItem.Id.ToString();
        entity.UserId = logItem.User.Id.ToString();
        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.ThreadUpdated = new ThreadUpdated
        {
            Before = before,
            After = after,
            AfterId = after.Id,
            BeforeId = before.Id
        };
    }

    private static ThreadInfo CreateThreadInfo(ThreadInfoRequest threadInfo)
    {
        return new ThreadInfo
        {
            Id = Guid.NewGuid(),
            Tags = threadInfo.Tags,
            Type = threadInfo.Type,
            ArchiveDuration = (ThreadArchiveDuration)threadInfo.ArchiveDuration,
            IsArchived = threadInfo.IsArchived,
            IsLocked = threadInfo.IsLocked,
            SlowMode = threadInfo.SlowMode,
            ThreadName = threadInfo.ThreadName!
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((ThreadUpdateAuditLogData)entry.Data).Thread.Id == request.ChannelId!.ToUlong();
}
