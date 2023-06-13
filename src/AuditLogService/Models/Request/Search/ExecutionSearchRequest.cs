namespace AuditLogService.Models.Request.Search;

public class ExecutionSearchRequest : IAdvancedSearchRequest
{
    public string? ActionName { get; set; }
    public bool? Success { get; set; }

    public int? DurationFrom { get; set; }
    public int? DurationTo { get; set; }

    public bool IsSet()
        => !string.IsNullOrEmpty(ActionName) || Success is not null || DurationFrom is not null || DurationTo is not null;
}
