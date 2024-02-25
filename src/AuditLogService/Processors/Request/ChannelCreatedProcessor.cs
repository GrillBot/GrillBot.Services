using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
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
        var logItem = await FindAuditLogAsync(request);
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

        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.UserId = logItem.User.Id.ToString();
        entity.DiscordId = logItem.Id.ToString();
        entity.ChannelCreated = new ChannelCreated
        {
            ChannelInfo = channelInfo,
            ChannelInfoId = channelInfo.Id
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((ChannelCreateAuditLogData)entry.Data).ChannelId == request.ChannelId!.ToUlong();
}
