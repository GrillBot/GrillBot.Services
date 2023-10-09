namespace AuditLogService.Models.Request.Search;

public class ApiSearchRequest : IAdvancedSearchRequest
{
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public string? PathTemplate { get; set; }
    public int? DurationFrom { get; set; }
    public int? DurationTo { get; set; }
    public string? Method { get; set; }
    public string? ApiGroupName { get; set; }
    public string? Identification { get; set; }

    public bool IsSet()
    {
        return !string.IsNullOrEmpty(ControllerName) || !string.IsNullOrEmpty(ActionName) || !string.IsNullOrEmpty(PathTemplate) || DurationFrom is not null || DurationTo is not null ||
               !string.IsNullOrEmpty(Method) || !string.IsNullOrEmpty(ApiGroupName) || !string.IsNullOrEmpty(Identification);
    }
}
