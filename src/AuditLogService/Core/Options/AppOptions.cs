namespace AuditLogService.Core.Options;

public class AppOptions
{
    public int MinimalItemsToArchive { get; set; }
    public int MaxItemsToArchivePerRun { get; set; }
    public int ExpirationMonths { get; set; }
}
