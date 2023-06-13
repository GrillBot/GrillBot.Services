using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class LogMessageProcessor : RequestProcessorBase
{
    public LogMessageProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var message = request.LogMessage!;
        entity.LogMessage = new LogMessage
        {
            Message = message.Message,
            Severity = message.Severity,
            SourceAppName = message.SourceAppName,
            Source = message.Source
        };

        return Task.CompletedTask;
    }
}
