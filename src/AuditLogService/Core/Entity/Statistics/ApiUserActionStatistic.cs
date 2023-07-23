using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class ApiUserActionStatistic : UserActionStatBase
{
    [StringLength(5)]
    public string ApiGroup { get; set; } = null!;

    public bool IsPublic { get; set; }

    public ApiUserActionStatistic()
    {
    }

    public ApiUserActionStatistic(string action, string userId, string apiGroup, bool isPublic) : base(action, userId)
    {
        ApiGroup = apiGroup;
        IsPublic = isPublic;
    }
}
