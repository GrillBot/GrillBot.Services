using GrillBot.Core.Database.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnverifyService.Core.Entity;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DiscordIdValueObject Id { get; set; }

    public TimeSpan? SelfUnverifyMinimalTime { get; set; }
    public bool IsBotAdmin { get; set; }
    public bool IsBot { get; set; }
    public string? Language { get; set; }
}
