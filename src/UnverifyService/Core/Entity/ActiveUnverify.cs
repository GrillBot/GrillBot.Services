using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UnverifyService.Core.Entity.Logs;

namespace UnverifyService.Core.Entity;

public class ActiveUnverify
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid LogSetId { get; set; }
    public UnverifyLogItem LogItem { get; set; } = null!;

    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }
}
