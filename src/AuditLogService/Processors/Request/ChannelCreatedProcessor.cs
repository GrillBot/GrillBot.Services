using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using ChannelInfo = AuditLogService.Core.Entity.ChannelInfo;

namespace AuditLogService.Processors.Request;

public class ChannelCreatedProcessor : RequestProcessorBase
{
    public ChannelCreatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.ChannelCreated)
            return;

        var auditLogs = await DiscordManager.GetAuditLogsAsync(entity.GuildId!.ToUlong(), actionType: ActionType.ChannelCreated);
        var logItem = auditLogs.FirstOrDefault(o => ((ChannelCreateAuditLogData)o.Data).ChannelId == request.ChannelId!.ToUlong());
        if (logItem is null)
        {
            entity.CanCreate = false;
            return;
        }

        var logData = (ChannelCreateAuditLogData)logItem.Data;
        var channelInfo = new ChannelInfo
        {
            Id = Guid.NewGuid(),
            Bitrate = logData.Bitrate,
            Position = request.ChannelInfo!.Position,
            Topic = request.ChannelInfo!.Topic,
            ChannelName = logData.ChannelName,
            ChannelType = logData.ChannelType,
            IsNsfw = logData.IsNsfw,
            SlowMode = logData.SlowModeInterval
        };

        entity.UserId = logItem.User.Id.ToString();
        entity.DiscordId = logItem.Id.ToString();
        entity.ChannelCreated = new ChannelCreated
        {
            ChannelInfo = channelInfo,
            ChannelInfoId = channelInfo.Id
        };
    }
}
