using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity.Statistics;

public class AuditLogDateStatistic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DateOnly Date { get; set; }

    public long Count { get; set; }

    public AuditLogDateStatistic()
    {
    }

    public AuditLogDateStatistic(DateOnly date)
    {
        Date = date;
    }
}
