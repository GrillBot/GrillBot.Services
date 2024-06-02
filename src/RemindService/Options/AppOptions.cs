namespace RemindService.Options;

public class AppOptions
{
    public const string FinishedUnsentMessageId = "0";

    public static Dictionary<int, string> PostponeHours { get; } = new Dictionary<int, string>
    {
        { 1, "one" },
        { 2, "two" },
        { 3, "three" },
        { 4, "four" },
        { 5, "five" },
        { 6, "six" },
        { 7, "seven" },
        { 8, "eight" },
        { 9, "nine" },
    };

    public TimeSpan MinimalTime { get; set; }
}
