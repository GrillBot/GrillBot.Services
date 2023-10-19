namespace AuditLogService.Models.Response.Statistics;

public class InteractionStatistics
{
    public Dictionary<string, long> ByDate { get; set; } = new();
    public List<StatisticItem> Commands { get; set; } = new();
}
