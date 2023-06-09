using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class UserJoinedRequest
{
    [Required]
    public int MemberCount { get; set; }
}
