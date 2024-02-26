using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class JobCompletedProcessor : RequestProcessorBase
{
    public JobCompletedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var job = request.JobExecution!;
        entity.Job = new JobExecution
        {
            Result = job.Result,
            EndAt = job.EndAt,
            JobName = job.JobName,
            StartAt = job.StartAt,
            WasError = job.WasError,
            StartUserId = job.StartUserId,
            Duration = (long)Math.Round((job.EndAt - job.StartAt).TotalMilliseconds),
            JobDate = DateOnly.FromDateTime(job.EndAt)
        };

        return Task.CompletedTask;
    }
}
