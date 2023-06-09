using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using ChannelInfo = AuditLogService.Core.Entity.ChannelInfo;

namespace AuditLogService.Processors.Request;

public class ChannelDeletedProcessor : RequestProcessorBase
{
    public ChannelDeletedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
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

        var logData = (ChannelDeleteAuditLogData)logItem.Data;
        var channelInfo = new ChannelInfo
        {
            Bitrate = logData.Bitrate,
            Id = Guid.NewGuid(),
            Position = request.ChannelInfo!.Position,
            Topic = request.ChannelInfo!.Topic,
            ChannelName = logData.ChannelName,
            ChannelType = logData.ChannelType,
            IsNsfw = logData.IsNsfw,
            SlowMode = logData.SlowModeInterval
        };

        entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        entity.DiscordId = logItem.Id.ToString();
        entity.UserId = logItem.User.Id.ToString();
        entity.ChannelDeleted = new ChannelDeleted
        {
            ChannelInfo = channelInfo,
            ChannelInfoId = channelInfo.Id
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((ChannelDeleteAuditLogData)entry.Data).ChannelId == request.ChannelId!.ToUlong();
}
