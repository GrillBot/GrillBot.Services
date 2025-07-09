namespace UnverifyService.Options;

public class ArchivationOptions
{
    public int MinimalCount { get; set; }
    public TimeSpan ExpirationMilestone { get; set; }
}
