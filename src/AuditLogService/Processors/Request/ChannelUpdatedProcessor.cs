using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using ChannelInfo = AuditLogService.Core.Entity.ChannelInfo;

namespace AuditLogService.Processors.Request;

public class ChannelUpdatedProcessor : RequestProcessorBase
{
    public ChannelUpdatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var logItem = await FindAuditLogAsync(request);
        if (logItem is null && !IsPositionOrApiUpdate(request.ChannelUpdated!))
        {
            entity.CanCreate = false;
            return;
        }

        var logData = logItem?.Data as ChannelUpdateAuditLogData;
        var before = CreateChannelInfo(logData?.Before, request.ChannelUpdated!.Before!);
        var after = CreateChannelInfo(logData?.After, request.ChannelUpdated!.After!);

        if (logItem is not null)
        {
            entity.DiscordId = logItem.Id.ToString();
            entity.UserId = logItem.User.Id.ToString();
            entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        }

        entity.ChannelUpdated = new ChannelUpdated
        {
            After = after,
            Before = before,
            AfterId = after.Id,
            BeforeId = before.Id
        };
    }

    private static ChannelInfo CreateChannelInfo(Discord.Rest.ChannelInfo? info, ChannelInfoRequest request)
    {
        var result = new ChannelInfo
        {
            Position = request.Position,
            Id = Guid.NewGuid(),
            Topic = request.Topic,
            Flags = request.Flags
        };

        if (info is null)
            return result;

        result.ChannelName = info.Value.Name;
        result.ChannelType = info.Value.ChannelType;
        result.IsNsfw = info.Value.IsNsfw;
        result.Bitrate = info.Value.Bitrate;
        result.SlowMode = info.Value.SlowModeInterval;
        return result;
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((ChannelUpdateAuditLogData)entry.Data).ChannelId == request.ChannelId.ToUlong();

    private static bool IsPositionOrApiUpdate(DiffRequest<ChannelInfoRequest> diff)
    {
        return diff.Before!.Position != diff.After!.Position ||
            diff.Before.Flags != diff.After.Flags;
    }
}
