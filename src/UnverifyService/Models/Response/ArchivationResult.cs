namespace UnverifyService.Models.Response;

public class ArchivationResult
{
    public string Content { get; set; } = null!;
    public List<ulong> UserIds { get; set; } = [];
    public List<ulong> GuildIds { get; set; } = [];
    public List<ulong> ChannelIds { get; set; } = [];
    public List<Guid> Ids { get; set; } = [];
    public int ItemsCount { get; set; }
    public Dictionary<string, int> PerType { get; set; } = [];
    public Dictionary<string, int> PerMonths { get; set; } = [];
}
