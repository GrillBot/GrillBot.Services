using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class MemberUpdatedRequest
{
    [Required]
    [StringLength(32)]
    public string UserId { get; set; } = null!;

    public DiffRequest<string?>? SelfUnverifyMinimalTime { get; set; }
    public DiffRequest<int?>? Flags { get; set; }

    public bool IsApiUpdate()
        => SelfUnverifyMinimalTime is not null || Flags is not null;
}
