namespace AuditLogService.Models.Events.Create;

public class MemberUpdatedRequest
{
    public string UserId { get; set; } = null!;

    public DiffRequest<string?>? SelfUnverifyMinimalTime { get; set; }
    public DiffRequest<int?>? Flags { get; set; }
    public DiffRequest<bool>? PointsDeactivated { get; set; }

    public bool IsApiUpdate()
        => SelfUnverifyMinimalTime is not null || Flags is not null || PointsDeactivated is not null;
}
