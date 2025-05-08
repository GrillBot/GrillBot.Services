using GrillBot.Core.Database.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmoteService.Core.Entity.Suggestions;

[Table("EmoteUserVotes", Schema = "suggestions")]
public class EmoteUserVote
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public DiscordIdValueObject UserId { get; set; }
    public Guid VoteSessionId { get; set; }
    public EmoteVoteSession VoteSession { get; set; } = null!;
    public bool IsApproved { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
