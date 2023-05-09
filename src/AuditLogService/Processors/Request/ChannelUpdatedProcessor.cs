using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
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
        if (entity.Type != LogType.ChannelUpdated)
            return;

        var auditLogs = await DiscordManager.GetAuditLogsAsync(entity.GuildId!.ToUlong(), actionType: ActionType.ChannelUpdated);
        var logItem = auditLogs.FirstOrDefault(o => ((ChannelUpdateAuditLogData)o.Data).ChannelId == request.ChannelId.ToUlong());
        if (logItem is null && request.ChannelUpdated!.Before!.Position == request.ChannelUpdated.After!.Position)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = logItem?.Data as ChannelUpdateAuditLogData;
        var before = CreateChannelInfo(logData?.Before, request.ChannelUpdated!.Before!);
        var after = CreateChannelInfo(logData?.After, request.ChannelUpdated!.After!);

        entity.DiscordId = logItem?.Id.ToString();
        entity.UserId = logItem?.User?.Id.ToString();
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
            Topic = request.Topic
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
}
