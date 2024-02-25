namespace AuditLogService.Models.Events.Create;

public class RoleDeletedRequest
{
    public string RoleId { get; set; } = null!;
}
