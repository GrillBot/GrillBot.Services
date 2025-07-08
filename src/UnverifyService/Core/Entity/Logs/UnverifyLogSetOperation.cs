using System.ComponentModel.DataAnnotations;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogSetOperation : UnverifyOperationBase
{
    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }
    public string? Reason { get; set; }

    [StringLength(85)]
    public string Language { get; set; } = null!;
    public bool KeepMutedRole { get; set; }

    public IList<UnverifyLogSetRole> Roles { get; set; } = [];
    public IList<UnverifyLogSetChannel> Channels { get; set; } = [];
}
