using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class ChannelInfoRequest
{
    public string? Topic { get; set; }

    [Required]
    public int Position { get; set; }
}
