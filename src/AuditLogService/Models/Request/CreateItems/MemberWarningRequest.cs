using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.CreateItems;

public class MemberWarningRequest
{
    public string Reason { get; set; } = null!;

    [DiscordId]
    [StringLength(32)]
    public string TargetId { get; set; } = null!;
}
