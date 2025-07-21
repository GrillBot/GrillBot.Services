namespace UnverifyService.Models.Request;

public class UnverifyRequest
{
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }

    public DateTime EndAtUtc { get; set; }
    public string? Reason { get; set; }
    public bool TestRun { get; set; }
    public bool IsSelfUnverify { get; set; }
    public List<string> RequiredKeepables { get; set; } = [];
}
