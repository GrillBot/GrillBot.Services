namespace EmoteService.Models.Response;

public class MergeStatisticsResult
{
    public int CreatedEmotesCount { get; set; }
    public int DeletedEmotesCount { get; set; }
    public int ModifiedEmotesCount { get; set; }
}
