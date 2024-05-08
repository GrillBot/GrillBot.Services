namespace RubbergodService.Models.Events.Karma;

public class KarmaUser
{
    public string MemberId { get; set; } = null!;
    public int Karma { get; set; }
    public int Positive { get; set; }
    public int Negative { get; set; }

    public KarmaUser()
    {
    }

    public KarmaUser(string memberId, int karma, int positive, int negative)
    {
        MemberId = memberId;
        Karma = karma;
        Positive = positive;
        Negative = negative;
    }
}
