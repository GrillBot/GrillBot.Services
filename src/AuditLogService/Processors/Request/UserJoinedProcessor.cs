using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class UserJoinedProcessor : RequestProcessorBase
{
    public UserJoinedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        entity.UserJoined = new UserJoined
        {
            MemberCount = request.UserJoined!.MemberCount
        };

        return Task.CompletedTask;
    }
}
