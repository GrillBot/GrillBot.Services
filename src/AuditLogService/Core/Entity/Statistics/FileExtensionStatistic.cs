using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity.Statistics;

public class FileExtensionStatistic
{
    [Key]
    [StringLength(255)]
    public string Extension { get; set; } = null!;

    public long Count { get; set; }

    public long Size { get; set; }

    public FileExtensionStatistic()
    {
    }

    public FileExtensionStatistic(string extension)
    {
        Extension = extension;
    }
}
