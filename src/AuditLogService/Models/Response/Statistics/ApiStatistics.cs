namespace AuditLogService.Models.Response.Statistics;

public class ApiStatistics
{
    public Dictionary<string, long> ByDateInternalApi { get; set; } = new();
    public Dictionary<string, long> ByDatePublicApi { get; set; } = new();
    public List<StatisticItem> Endpoints { get; set; } = new();
}
