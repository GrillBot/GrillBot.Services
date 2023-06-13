namespace AuditLogService.Models.Response.Statistics;

public class StatisticItem
{
    public string Key { get; set; } = null!;
    public DateTime Last { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int MinDuration { get; set; }
    public int MaxDuration { get; set; }
    public int TotalDuration { get; set; }
    public int LastRunDuration { get; set; }

    public int SuccessRate
    {
        get
        {
            var sum = SuccessCount + FailedCount;
            return sum == 0 ? 0 : (int)Math.Round((double)SuccessCount / sum * 100);
        }
    }

    public int AvgDuration
    {
        get
        {
            var sum = SuccessCount + FailedCount;
            return sum == 0 ? 0 : Convert.ToInt32(Math.Round(TotalDuration / (double)sum));
        }
    }
}
