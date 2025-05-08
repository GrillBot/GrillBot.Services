using GrillBot.Core.Database.ValueObjects;
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
    public DateTime? KilledAtUtc { get; set; }
    public DiscordIdValueObject? VoteMessageId { get; set; }
    public bool IsClosed { get; set; }

    public IList<EmoteUserVote> UserVotes { get; set; } = [];

    public bool Running()
        => KilledAtUtc == null && ExpectedVoteEndAtUtc > DateTime.UtcNow;

    public int UpVotes()
        => UserVotes.Count(o => o.IsApproved);

    public int DownVotes()
        => UserVotes.Count(o => !o.IsApproved);

    public bool IsCommunityApproved()
        => KilledAtUtc is null && UpVotes() > DownVotes();
}
