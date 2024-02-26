namespace AuditLogService.Models.Events.Recalculation;

public class InteractionRecalculationData
{
    public string Name { get; set; } = null!;
    public string ModuleName { get; set; } = null!;
    public string MethodName { get; set; } = null!;
    public bool IsSuccess { get; set; }
    public DateOnly EndDate { get; set; }
    public string UserId { get; set; } = null!;

    public override string ToString()
        => $"{Name} ({ModuleName} / {MethodName}) {IsSuccess} {EndDate:yyyyMMdd} {UserId}";
}
