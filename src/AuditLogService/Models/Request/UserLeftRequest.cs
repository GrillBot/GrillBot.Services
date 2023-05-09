using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace AuditLogService.Models.Request;

public class UserLeftRequest : UserJoinedRequest
{
    [Required]
    [StringLength(32)]
    [DiscordId]
    public string UserId { get; set; } = null!;
}
