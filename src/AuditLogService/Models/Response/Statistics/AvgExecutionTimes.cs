namespace AuditLogService.Models.Response.Statistics;

public class AvgExecutionTimes
{
    public Dictionary<string, double> InternalApi { get; set; } = [];
    public Dictionary<string, double> ExternalApi { get; set; } = [];
    public Dictionary<string, double> Jobs { get; set; } = [];
    public Dictionary<string, double> Interactions { get; set; } = [];
}
