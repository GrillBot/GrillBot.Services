namespace AuditLogService.Models.Request;

public class ChannelInfoRequest
{
    public string? Topic { get; set; }
    public int Position { get; set; }
    public int Flags { get; set; }
}
