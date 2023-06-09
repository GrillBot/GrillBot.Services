using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request;

public class ApiRequestRequest
{
    [Required]
    [StringLength(128)]
    public string ControllerName { get; set; } = null!;

    [Required]
    [StringLength(128)]
    public string ActionName { get; set; } = null!;

    [Required]
    public DateTime StartAt { get; set; }

    [Required]
    public DateTime EndAt { get; set; }

    [Required]
    [StringLength(10)]
    public string Method { get; set; } = null!;

    [Required]
    public string TemplatePath { get; set; } = null!;

    [Required]
    public string Path { get; set; } = null!;

    [Required]
    public Dictionary<string, string> Parameters { get; set; } = new();

    [Required]
    [StringLength(10)]
    public string Language { get; set; } = null!;

    [Required]
    [StringLength(5)]
    public string ApiGroupName { get; set; } = null!;

    [Required]
    public Dictionary<string, string> Headers { get; set; } = new();

    [Required]
    [StringLength(512)]
    public string Identification { get; set; } = null!;

    [Required]
    [StringLength(128)]
    public string Ip { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string Result { get; set; } = null!;
}
