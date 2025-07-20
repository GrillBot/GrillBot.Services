namespace UnverifyService.Options;

public class AppOptions
{
    public ArchivationOptions Archivation { get; set; } = new();
    public int MaxAllowedKeepables { get; set; }
    public TimeSpan UnverifyMinimalTime { get; set; }
    public TimeSpan SelfUnverifyMinimalTime { get; set; }
}
