using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using Discord;
using GrillBot.Core.Extensions;

namespace AuditLogService.Processors.Request.Abstractions;

public abstract class RequestProcessorBase
{
    protected DiscordManager DiscordManager { get; }

    protected RequestProcessorBase(IServiceProvider serviceProvider)
    {
        DiscordManager = serviceProvider.GetRequiredService<DiscordManager>();
    }

    public abstract Task ProcessAsync(LogItem entity, LogRequest request);

    private static ActionType ConvertActionType(LogType type)
    {
        return type switch
        {
            LogType.ChannelCreated => ActionType.ChannelCreated,
            LogType.Unban => ActionType.Unban,
            LogType.OverwriteCreated => ActionType.OverwriteCreated,
            LogType.OverwriteDeleted => ActionType.OverwriteDeleted,
            LogType.OverwriteUpdated => ActionType.OverwriteUpdated,
            LogType.MemberRoleUpdated => ActionType.MemberRoleUpdated,
            LogType.ChannelDeleted => ActionType.ChannelDeleted,
            LogType.ChannelUpdated => ActionType.ChannelUpdated,
            LogType.EmoteDeleted => ActionType.EmojiDeleted,
            LogType.GuildUpdated => ActionType.GuildUpdated,
            LogType.MemberUpdated => ActionType.MemberUpdated,
            LogType.MessageDeleted => ActionType.MessageDeleted,
            LogType.ThreadDeleted => ActionType.ThreadDelete,
            LogType.ThreadUpdated => ActionType.ThreadUpdate,
            _ => throw new ArgumentException($"Unsupported type ({type})", nameof(type))
        };
    }

    protected virtual async Task<List<IAuditLogEntry>> FindAuditLogsAsync(LogRequest request, ActionType? type = null)
    {
        var actionType = type ?? ConvertActionType(request.Type);
        var auditLogs = await DiscordManager.GetAuditLogsAsync(request.GuildId!.ToUlong(), actionType: actionType);

        return auditLogs
            .Where(o => IsValidAuditLogItem(o, request))
            .ToList();
    }

    protected virtual async Task<IAuditLogEntry?> FindAuditLogAsync(LogRequest request, ActionType? type = null)
    {
        var logs = await FindAuditLogsAsync(request, type);
        return logs.Count == 0 ? null : logs[0];
    }

    protected virtual bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request) => false;
}
