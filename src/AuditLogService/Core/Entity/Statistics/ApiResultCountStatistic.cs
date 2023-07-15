using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class ApiResultCountStatistic
{
    [StringLength(255)]
    public string Result { get; set; } = null!;

    [StringLength(5)]
    public string ApiGroup { get; set; } = null!;

    public long Count { get; set; }
}
