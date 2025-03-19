﻿using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class LogMessageProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
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
