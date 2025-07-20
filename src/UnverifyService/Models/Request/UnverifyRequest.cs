namespace UnverifyService.Models.Request;

public class UnverifyRequest
{
    /// <summary>
    /// List of guild users (user id) grouped by guild id.
    /// </summary>
    public Dictionary<ulong, List<ulong>> Users { get; set; } = [];

    public DateTime EndAtUtc { get; set; }
    public string? Reason { get; set; }
    public bool TestRun { get; set; }
    public bool IsSelfUnverify { get; set; }
    public List<string> RequiredKeepables { get; set; } = [];
}
