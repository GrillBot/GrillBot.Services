using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity;

[Index(nameof(InteractionDate))]
public class InteractionCommand : ChildEntityBase
{
    public string Name { get; set; } = null!;
    public string ModuleName { get; set; } = null!;
    public string MethodName { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public List<InteractionCommandParameter>? Parameters { get; set; }

    public bool HasResponded { get; set; }
    public bool IsValidToken { get; set; }
    public bool IsSuccess { get; set; }
    public int? CommandError { get; set; }
    public string? ErrorReason { get; set; }
    public int Duration { get; set; }
    public string? Exception { get; set; }
    public string Locale { get; set; } = "cs";
    public DateTime EndAt { get; set; }
    public DateOnly InteractionDate { get; set; }
}
