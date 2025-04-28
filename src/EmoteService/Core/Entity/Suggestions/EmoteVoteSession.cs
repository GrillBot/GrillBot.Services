using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmoteService.Core.Entity.Suggestions;

[Table("EmoteVoteSessions", Schema = "suggestions")]
public class EmoteVoteSession
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public EmoteSuggestion Suggestion { get; set; } = null!;

    public DateTime VoteStartedAtUtc { get; set; }
    public DateTime ExpectedVoteEndAtUtc { get; set; }

    public IList<EmoteUserVote> UserVotes { get; set; } = [];
}
