using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class ApiDateCountStatistic : DateCountStatistic
{
    [StringLength(5)]
    public string ApiGroup { get; set; } = null!;

    public ApiDateCountStatistic()
    {
    }

    public ApiDateCountStatistic(DateOnly date, string apiGroup) : base(date)
    {
        ApiGroup = apiGroup;
    }
}
