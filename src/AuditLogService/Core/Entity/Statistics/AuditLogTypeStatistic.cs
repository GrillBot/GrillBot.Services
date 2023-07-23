using AuditLogService.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity.Statistics;

public class AuditLogTypeStatistic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public LogType Type { get; set; }

    public long Count { get; set; }

    public AuditLogTypeStatistic()
    {
    }

    public AuditLogTypeStatistic(LogType type)
    {
        Type = type;
    }
}
