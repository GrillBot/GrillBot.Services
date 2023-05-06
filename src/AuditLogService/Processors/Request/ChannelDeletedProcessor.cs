﻿using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using ChannelInfo = AuditLogService.Core.Entity.ChannelInfo;

namespace AuditLogService.Processors.Request;

public class ChannelDeletedProcessor : RequestProcessorBase
{
    public ChannelDeletedProcessor(DiscordManager discordManager) : base(discordManager)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.ChannelDeleted)
            return;

        var auditLogs = await DiscordManager.GetAuditLogsAsync(entity.GuildId!.ToUlong(), actionType: ActionType.ChannelDeleted);
        var logItem = auditLogs.FirstOrDefault(o => ((ChannelDeleteAuditLogData)o.Data).ChannelId == request.ChannelId!.ToUlong());
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

        entity.DiscordId = logItem.Id.ToString();
        entity.UserId = logItem.User.Id.ToString();
        entity.ChannelDeleted = new ChannelDeleted
        {
            ChannelInfo = channelInfo,
            ChannelInfoId = channelInfo.Id
        };
    }
}
