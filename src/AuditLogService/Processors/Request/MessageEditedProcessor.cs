using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class MessageEditedProcessor : RequestProcessorBase
{
    public MessageEditedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        entity.MessageEdited = new MessageEdited
        {
            ContentAfter = request.MessageEdited!.ContentAfter,
            ContentBefore = request.MessageEdited.ContentBefore,
            JumpUrl = request.MessageEdited.JumpUrl
        };

        return Task.CompletedTask;
    }
}
