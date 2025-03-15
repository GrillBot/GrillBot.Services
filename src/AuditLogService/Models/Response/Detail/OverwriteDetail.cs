using Discord;

namespace AuditLogService.Models.Response.Detail;

public class OverwriteDetail
{
    public string TargetId { get; set; } = null!;
    public PermissionTarget TargetType { get; set; }

    public List<string> Allow { get; set; } = new();
    public List<string> Deny { get; set; } = new();
}
