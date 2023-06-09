namespace AuditLogService.Models.Request.Search;

public class TextSearchRequest : IAdvancedSearchRequest
{
    public string? Text { get; set; } = null!;

    public bool IsSet()
        => !string.IsNullOrEmpty(Text);
}
