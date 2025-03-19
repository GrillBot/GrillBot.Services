using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Events.Recalculation;
using GrillBot.Core.Extensions;
using GrillBot.Core.RabbitMQ.V2.Publisher;

namespace AuditLogService.Managers;

public class DataRecalculationManager(IRabbitPublisher _publisher)
{
    public async Task EnqueueRecalculationAsync(List<LogItem> items)
    {
        var batches = CreateBatches(items);
        await _publisher.PublishAsync("AuditLog", batches, "Recalculation");
    }

    private static List<RecalculationPayload> CreateBatches(List<LogItem> items)
    {
        var result = new List<RecalculationPayload>();

        foreach (var item in items)
        {
            if (!result.Exists(o => IsEqual(o, item)))
                result.Add(CreateNewPayload(item));
        }

        return result;
    }

    private static bool IsEqual(RecalculationPayload payload, LogItem item)
    {
        if (item.Type != payload.Type)
            return false;

        if (payload.Api is not null)
        {
            var request = item.ApiRequest;
            if (request is null)
                return false;

            return request.RequestDate == payload.Api.RequestDate &&
                request.Method == payload.Api.Method &&
                request.TemplatePath == payload.Api.TemplatePath &&
                request.ApiGroupName == payload.Api.ApiGroupName &&
                (item.UserId ?? request.Identification) == payload.Api.Identification;
        }

        if (payload.Interaction is not null)
        {
            var interaction = item.InteractionCommand!;
            if (interaction is null)
                return false;

            return interaction.Name == payload.Interaction.Name &&
                interaction.ModuleName == payload.Interaction.ModuleName &&
                interaction.MethodName == payload.Interaction.MethodName &&
                interaction.IsSuccess == payload.Interaction.IsSuccess &&
                interaction.EndAt.Date.ToDateOnly() == payload.Interaction.EndDate &&
                item.UserId == payload.Interaction.UserId;
        }

        if (payload.Job is not null)
        {
            var job = item.Job!;
            if (job is null)
                return false;

            return job.JobName == payload.Job.JobName &&
                job.JobDate == payload.Job.JobDate;
        }

        return true;
    }

    private static RecalculationPayload CreateNewPayload(LogItem item)
    {
        var payload = new RecalculationPayload(item.Type);

        switch (item.Type)
        {
            case LogType.Api when item.ApiRequest is not null:
                var request = item.ApiRequest;
                payload.Api = new ApiRecalculationData
                {
                    ApiGroupName = request.ApiGroupName,
                    Identification = item.UserId ?? request.Identification,
                    Method = request.Method,
                    RequestDate = request.RequestDate,
                    TemplatePath = request.TemplatePath
                };
                break;
            case LogType.InteractionCommand when item.InteractionCommand is not null:
                var interaction = item.InteractionCommand;
                payload.Interaction = new InteractionRecalculationData
                {
                    EndDate = interaction.EndAt.Date.ToDateOnly(),
                    IsSuccess = interaction.IsSuccess,
                    MethodName = interaction.MethodName,
                    ModuleName = interaction.ModuleName,
                    Name = interaction.Name,
                    UserId = item.UserId!
                };
                break;
            case LogType.JobCompleted when item.Job is not null:
                var job = item.Job;
                payload.Job = new JobRecalculationData
                {
                    JobDate = job.JobDate,
                    JobName = job.JobName
                };
                break;
        }

        return payload;
    }
}
