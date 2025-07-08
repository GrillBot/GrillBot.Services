namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogUpdateOperation : UnverifyOperationBase
{
    public DateTime NewStartAtUtc { get; set; }
    public DateTime NewEndAtUtc { get; set; }
    public string? Reason { get; set; }
}
