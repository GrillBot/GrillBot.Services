namespace RubbergodService.Core.Models;

public class UserKarma
{
    public User User { get; set; } = null!;
    public int Negative { get; set; }
    public int Positive { get; set; }
    public int Value { get; set; }
    public int Position { get; set; }
}
