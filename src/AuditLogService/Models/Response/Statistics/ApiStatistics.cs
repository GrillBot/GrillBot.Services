namespace AuditLogService.Models.Response.Statistics;

public class ApiStatistics
{
    public Dictionary<DateOnly, int> DailyInternalApi { get; set; } = [];
    public Dictionary<DateOnly, int> DailyPublicApi { get; set; } = [];
    public List<StatisticItem> Endpoints { get; set; } = [];
}
