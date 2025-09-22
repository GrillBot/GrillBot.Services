using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Entity;

[Index(nameof(Method), nameof(TemplatePath), nameof(ApiGroupName), nameof(Identification))]
[Index(nameof(ApiGroupName), nameof(RequestDate))]
[Index(nameof(ApiGroupName), nameof(EndAt))]
public class ApiRequest : ChildEntityBase
{
    [StringLength(128)]
    public string ControllerName { get; set; } = null!;

    [StringLength(128)]
    public string ActionName { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    [StringLength(10)]
    public string Method { get; set; } = null!;

    public string TemplatePath { get; set; } = null!;

    public string Path { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public Dictionary<string, string>? Parameters { get; set; }

    [StringLength(10)]
    public string Language { get; set; } = null!;

    [StringLength(5)]
    public string ApiGroupName { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public Dictionary<string, string>? Headers { get; set; }

    [StringLength(512)]
    public string Identification { get; set; } = null!;

    [StringLength(64)]
    public string Ip { get; set; } = null!;

    [StringLength(128)]
    public string Result { get; set; } = null!;

    public bool IsSuccess { get; set; }

    public DateOnly RequestDate { get; set; }

    [StringLength(32)]
    public string? Role { get; set; }

    [StringLength(64)]
    public string? ForwardedIp { get; set; }

    public int Duration { get; set; }
}
