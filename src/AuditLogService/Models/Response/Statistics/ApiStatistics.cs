namespace AuditLogService.Models.Response.Statistics;

public class ApiStatistics
{
    public Dictionary<string, long> ByDateInternalApi { get; set; } = [];
    public Dictionary<string, long> ByDatePublicApi { get; set; } = [];
    public Dictionary<DateOnly, int> DailyInternalApi { get; set; } = [];
    public Dictionary<DateOnly, int> DailyPublicApi { get; set; } = [];
    public List<StatisticItem> Endpoints { get; set; } = [];
}
