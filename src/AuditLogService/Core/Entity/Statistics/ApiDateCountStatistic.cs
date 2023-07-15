using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class ApiDateCountStatistic
{
    public DateOnly Date { get; set; }
    public long Count { get; set; }

    [StringLength(5)]
    public string ApiGroup { get; set; } = null!;
}
