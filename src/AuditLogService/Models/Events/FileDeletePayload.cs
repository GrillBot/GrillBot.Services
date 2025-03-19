namespace AuditLogService.Models.Events;

public class FileDeletePayload
{
    public string Filename { get; set; } = null!;

    public FileDeletePayload()
    {
    }

    public FileDeletePayload(string filename)
    {
        Filename = filename;
    }
}
