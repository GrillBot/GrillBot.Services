using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class UserJoinedProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        entity.UserJoined = new UserJoined
        {
            MemberCount = request.UserJoined!.MemberCount
        };

        return Task.CompletedTask;
    }
}
