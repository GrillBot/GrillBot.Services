namespace AuditLogService.Models.Events.Recalculation;

public class JobRecalculationData
{
    public DateOnly JobDate { get; set; }
    public string JobName { get; set; } = null!;

    public override string ToString()
        => $"{JobDate:yyyyMMdd} {JobName}";
}
