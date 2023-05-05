using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class UserJoinedRequest
{
    [Required]
    public int MemberCount { get; set; }
}
