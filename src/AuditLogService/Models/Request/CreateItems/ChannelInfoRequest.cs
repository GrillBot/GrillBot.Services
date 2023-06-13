namespace AuditLogService.Models.Request.CreateItems;

public class ChannelInfoRequest
{
    public string? Topic { get; set; }
    public int Position { get; set; }
    public int Flags { get; set; }
}
