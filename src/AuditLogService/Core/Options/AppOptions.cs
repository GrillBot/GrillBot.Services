namespace AuditLogService.Core.Options;

public class AppOptions
{
    public int ItemsToArchive { get; set; }
    public int ExpirationMonths { get; set; }
}
