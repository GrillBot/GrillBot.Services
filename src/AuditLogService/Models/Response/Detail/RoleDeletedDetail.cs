namespace AuditLogService.Models.Response.Detail;

public class RoleDeletedDetail
{
    public string RoleId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public bool IsMentionable { get; set; }
    public bool IsHoisted { get; set; }
    public List<string> Permissions { get; set; } = new();
    public string? IconId { get; set; }
}
