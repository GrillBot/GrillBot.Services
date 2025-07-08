using System.ComponentModel.DataAnnotations;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogRemoveOperation : UnverifyOperationBase
{
    public bool IsFromWeb { get; set; }

    [StringLength(85)]
    public string? Language { get; set; }
    public bool Force { get; set; }

    public IList<UnverifyLogRemoveRole> Roles { get; set; } = [];
    public IList<UnverifyLogRemoveChannel> Channels { get; set; } = [];
}
