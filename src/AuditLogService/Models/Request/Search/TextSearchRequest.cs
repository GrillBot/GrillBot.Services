using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.Search;

public class TextSearchRequest : IAdvancedSearchRequest
{
    public string? Text { get; set; } = null!;

    [StringLength(100)]
    public string? SourceAppName { get; set; }

    [StringLength(512)]
    public string? Source { get; set; }

    public bool IsSet()
        => !string.IsNullOrEmpty(Text) || !string.IsNullOrEmpty(SourceAppName) || !string.IsNullOrEmpty(Source);
}
