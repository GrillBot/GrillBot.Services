using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class JobCompletedProcessor : RequestProcessorBase
{
    public JobCompletedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        entity.Job = new JobExecution
        {
            Result = request.JobExecution!.Result,
            EndAt = request.JobExecution.EndAt,
            JobName = request.JobExecution.JobName,
            StartAt = request.JobExecution.StartAt,
            WasError = request.JobExecution.WasError,
            StartUserId = request.JobExecution.StartUserId
        };

        return Task.CompletedTask;
    }
}
