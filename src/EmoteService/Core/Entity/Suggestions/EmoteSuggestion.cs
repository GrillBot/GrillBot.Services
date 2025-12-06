using GrillBot.Core.Database.ValueObjects;
using GrillBot.Models.Events.Messages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmoteService.Core.Entity.Suggestions;

[Table("EmoteSuggestions", Schema = "suggestions")]
public class EmoteSuggestion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DiscordIdValueObject FromUserId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    public DateTime SuggestedAtUtc { get; set; }
    public byte[] Image { get; set; } = null!;
    public DiscordIdValueObject GuildId { get; set; }
    public DiscordIdValueObject SuggestionMessageId { get; set; }
    public bool ApprovedForVote { get; set; }
    public DiscordIdValueObject? ApprovalByUserId { get; set; }
    public DateTime? ApprovalSetAtUtc { get; set; }

    [StringLength(4000)]
    public string ReasonForAdd { get; set; } = null!;

    public bool IsAnimated { get; set; }

    public EmoteVoteSession? VoteSession { get; set; }

    public DiscordMessageFile CreateImageFile()
        => new($"{Id}.{(IsAnimated ? "gif" : "png")}", false, Image);
}
