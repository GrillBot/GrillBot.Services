namespace AuditLogService.Models.Response;

public class ArchivationResult
{
    public string Content { get; set; } = null!;
    public List<string> Files { get; set; } = [];
    public List<string> UserIds { get; set; } = [];
    public List<string> GuildIds { get; set; } = [];
    public List<string> ChannelIds { get; set; } = [];
    public List<Guid> Ids { get; set; } = [];
    public int ItemsCount { get; set; }
    public long TotalFilesSize { get; set; }
    public Dictionary<string, long> PerType { get; set; } = [];
    public Dictionary<string, long> PerMonths { get; set; } = [];
}
