using GrillBot.Core.RabbitMQ.V2.Messages;

namespace AuditLogService.Models.Events;

public class FileDeletePayload : IRabbitMessage
{
    public string Topic => "AuditLog";
    public string Queue => "FileDelete";

    public string Filename { get; set; } = null!;

    public FileDeletePayload()
    {
    }

    public FileDeletePayload(string filename)
    {
        Filename = filename;
    }
}
