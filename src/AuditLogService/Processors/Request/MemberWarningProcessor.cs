using AuditLogService.Core.Entity;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class MemberWarningProcessor : RequestProcessorBase
{
    public MemberWarningProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        entity.MemberWarning = new MemberWarning
        {
            Reason = request.MemberWarning!.Reason,
            TargetId = request.MemberWarning!.TargetId
        };

        return Task.CompletedTask;
    }
}
