using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;

namespace AuditLogService.Processors.Request;

public class LogMessageProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var message = request.LogMessage!;

        entity.LogMessage = new Core.Entity.LogMessage
        {
            Message = message.Message,
            Severity = entity.Type switch
            {
                LogType.Info => LogSeverity.Info,
                LogType.Warning => LogSeverity.Warning,
                LogType.Error => LogSeverity.Error,
                _ => throw new NotSupportedException()
            },
            SourceAppName = message.SourceAppName,
            Source = message.Source
        };

        return Task.CompletedTask;
    }
}
