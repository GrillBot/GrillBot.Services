using AuditLogService.Core.Enums;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace AuditLogService.Models.Events.Recalculation;

public class RecalculationPayload : IRabbitMessage
{
    public string Topic => "AuditLog";
    public string Queue => "UserRecalculation";

    public LogType Type { get; set; }
    public InteractionRecalculationData? Interaction { get; set; }
    public ApiRecalculationData? Api { get; set; }
    public JobRecalculationData? Job { get; set; }
    public int FilesCount { get; set; }

    public RecalculationPayload()
    {
    }

    public RecalculationPayload(LogType type, InteractionRecalculationData? interaction = null, ApiRecalculationData? api = null, JobRecalculationData? job = null, int filesCount = 0)
    {
        Type = type;
        Interaction = interaction;
        Api = api;
        Job = job;
        FilesCount = filesCount;
    }

    public override string ToString()
    {
        var item = Interaction?.ToString() ?? Api?.ToString() ?? Job?.ToString();

        if (string.IsNullOrEmpty(item) && FilesCount > 0)
            item = $"Files:{FilesCount}";

        return $"{Type} ({item})";
    }
}
