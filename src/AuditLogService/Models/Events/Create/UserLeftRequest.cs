namespace AuditLogService.Models.Events.Create;

public class UserLeftRequest : UserJoinedRequest
{
    public string UserId { get; set; } = null!;
}
