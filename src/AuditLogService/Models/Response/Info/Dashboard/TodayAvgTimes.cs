namespace AuditLogService.Models.Response.Info.Dashboard;

public class TodayAvgTimes
{
    public long PrivateApi { get; set; }
    public long PublicApi { get; set; }
    public long Interactions { get; set; }
    public long Jobs { get; set; }
}
