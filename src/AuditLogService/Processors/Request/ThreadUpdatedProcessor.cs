using AuditLogService.Core.Discord;
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

        var forum = await FindForumAsync(request);
        if (forum is null)
        {
            entity.CanCreate = false;
            return;
        }

        var before = CreateThreadInfo(request, request.ThreadUpdated!.Before!, forum);
        var after = CreateThreadInfo(request, request.ThreadUpdated!.After!, forum);

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

    private ThreadInfo CreateThreadInfo(LogRequest request, ThreadInfoRequest threadInfo, IForumChannel forum)
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
            ThreadName = threadInfo.ThreadName
        };
    }

    private async Task<IForumChannel?> FindForumAsync(LogRequest request)
    {
        var forumId = request.ThreadUpdated?.Before?.ParentChannelId ?? request.ThreadUpdated?.After?.ParentChannelId;
        if (string.IsNullOrEmpty(forumId))
            return null;

        var guild = await DiscordManager.GetGuildAsync(request.GuildId!.ToUlong());
        if (guild is null)
            return null;

        var forum = await guild.GetChannelAsync(request.ThreadUpdated!.Before!.ParentChannelId.ToUlong());
        return forum as IForumChannel;
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((ThreadUpdateAuditLogData)entry.Data).Thread.Id == request.ChannelId!.ToUlong();
}
