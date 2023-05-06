using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;

namespace AuditLogService.Processors.Request;

public class LogMessageProcessor : RequestProcessorBase
{
    public LogMessageProcessor(DiscordManager discordManager) : base(discordManager)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.Error && entity.Type != LogType.Info && entity.Type != LogType.Warning)
            return Task.CompletedTask;

        var message = request.LogMessage!;
        entity.LogMessage = new LogMessage
        {
            Message = message.Message,
            Severity = message.Severity
        };

        return Task.CompletedTask;
    }
}
