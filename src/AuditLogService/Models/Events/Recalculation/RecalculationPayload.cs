using AuditLogService.Core.Enums;
using GrillBot.Core.RabbitMQ;

namespace AuditLogService.Models.Events.Recalculation;

public class RecalculationPayload : IPayload
{
    public string QueueName => "audit:recalculation";

    public LogType Type { get; set; }
    public InteractionRecalculationData? Interaction { get; set; }
    public ApiRecalculationData? Api { get; set; }
    public JobRecalculationData? Job { get; set; }

    public RecalculationPayload()
    {
    }

    public RecalculationPayload(LogType type, InteractionRecalculationData? interaction = null, ApiRecalculationData? api = null, JobRecalculationData? job = null)
    {
        Type = type;
        Interaction = interaction;
        Api = api;
        Job = job;
    }

    public override string ToString()
    {
        var item = Interaction?.ToString() ?? Api?.ToString() ?? Job?.ToString();
        return $"{Type} ({item})";
    }
}
