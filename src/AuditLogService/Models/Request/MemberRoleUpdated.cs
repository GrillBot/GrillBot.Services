using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace AuditLogService.Models.Request;

public class MemberRoleUpdated
{
    /// <summary>
    /// ID of audit log record in the discord.
    /// </summary>
    [Required]
    [StringLength(32)]
    [DiscordId]
    public string DiscordLogId { get; set; } = null!;
}
