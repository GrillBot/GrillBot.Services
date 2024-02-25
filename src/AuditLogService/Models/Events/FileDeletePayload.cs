using GrillBot.Core.RabbitMQ;

namespace AuditLogService.Models.Events;

public class FileDeletePayload : IPayload
{
    public string QueueName => "audit:file_delete_request";

    public string Filename { get; set; } = null!;

    public FileDeletePayload()
    {
    }

    public FileDeletePayload(string filename)
    {
        Filename = filename;
    }
}
