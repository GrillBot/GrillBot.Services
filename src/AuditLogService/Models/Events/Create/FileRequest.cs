namespace AuditLogService.Models.Events.Create;

public class FileRequest
{
    public string Filename { get; set; } = null!;
    public string? Extension { get; set; }
    public long Size { get; set; }
}
