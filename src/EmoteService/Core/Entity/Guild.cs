using GrillBot.Core.Database.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmoteService.Core.Entity;

public class Guild
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DiscordIdValueObject GuildId { get; set; }

    public DiscordIdValueObject SuggestionChannelId { get; set; }
    public DiscordIdValueObject VoteChannelId { get; set; }
    public TimeSpan VoteTime { get; set; }
}
