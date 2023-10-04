using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request.CreateItems;
using Discord;
using Discord.Net;
using GrillBot.Core.Extensions;
using System.Net;

namespace AuditLogService.Processors.Request.Abstractions;

public abstract class RequestProcessorBase
{
    public Dictionary<LogSeverity, List<string>> ProcessingMessages { get; } = new();

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
            LogType.RoleDeleted => ActionType.RoleDeleted,
            _ => throw new ArgumentException($"Unsupported type ({type})", nameof(type))
        };
    }

    protected virtual async Task<List<IAuditLogEntry>> FindAuditLogsAsync(LogRequest request, ActionType? type = null)
    {
        try
        {
            var actionType = type ?? ConvertActionType(request.Type);
            var auditLogs = await DiscordManager.GetAuditLogsAsync(request.GuildId!.ToUlong(), actionType: actionType);

            return auditLogs
                .Where(o => IsValidAuditLogItem(o, request))
                .ToList();
        }
        catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.GatewayTimeout)
        {
            return new List<IAuditLogEntry>();
        }
    }

    protected virtual async Task<IAuditLogEntry?> FindAuditLogAsync(LogRequest request, ActionType? type = null)
    {
        var logs = await FindAuditLogsAsync(request, type);
        return logs.Count == 0 ? null : logs[0];
    }

    protected virtual bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request) => false;

    protected void AddWarning(string message)
        => AddProcessingMessage(LogSeverity.Warning, message);

    private void AddProcessingMessage(LogSeverity severity, string message)
    {
        if (!ProcessingMessages.ContainsKey(severity))
            ProcessingMessages.Add(severity, new List<string>());
        ProcessingMessages[severity].Add(message);
    }
}
