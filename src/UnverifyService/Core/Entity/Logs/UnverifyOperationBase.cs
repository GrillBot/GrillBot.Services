using System.ComponentModel.DataAnnotations;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyOperationBase
{
    [Key]
    public Guid LogItemId { get; set; }

    public UnverifyLogItem LogItem { get; set; } = null!;
}
