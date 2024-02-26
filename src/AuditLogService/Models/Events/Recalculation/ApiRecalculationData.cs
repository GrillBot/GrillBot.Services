namespace AuditLogService.Models.Events.Recalculation;

public class ApiRecalculationData
{
    public DateOnly RequestDate { get; set; }
    public string Method { get; set; } = null!;
    public string TemplatePath { get; set; } = null!;
    public string ApiGroupName { get; set; } = null!;
    public string Identification { get; set; } = null!;

    public override string ToString()
        => $"{RequestDate:yyyyMMdd} {Method} {TemplatePath} {ApiGroupName} {Identification}";
}
