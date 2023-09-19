using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity.Statistics;

public class DatabaseStatistic
{
    [Key]
    [StringLength(1024)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string TableName { get; set; } = null!;

    public long RecordsCount { get; set; }

    public DatabaseStatistic()
    {
    }

    public DatabaseStatistic(string tableName)
    {
        TableName = tableName;
    }
}
