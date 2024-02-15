using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PointsService.Core.Entity;

[Index(nameof(MergedCount))]
[Index(nameof(CreatedAt))]
[Index(nameof(GuildId), nameof(MessageId))]
[Index(nameof(GuildId), nameof(UserId))]
public class Transaction
{
    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    [StringLength(30)]
    public string UserId { get; set; } = null!;

    [StringLength(30)]
    public string MessageId { get; set; } = null!;

    [StringLength(100)]
    public string ReactionId { get; set; } = "";

    public int MergedCount { get; set; }
    public DateTime? MergeRangeFrom { get; set; }
    public DateTime? MergeRangeTo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Value { get; set; }

    [NotMapped]
    public bool IsReaction
        => !string.IsNullOrEmpty(ReactionId);
}
