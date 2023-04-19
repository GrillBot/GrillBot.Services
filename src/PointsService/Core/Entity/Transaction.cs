using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PointsService.Core.Entity;

[Index(nameof(MergedCount), Name = "IX_Transactions_MergedCount", IsUnique = false)]
[Index(nameof(CreatedAt), Name = "IX_Transactions_CreatedAt", IsUnique = false)]
[Index(nameof(GuildId), Name = "IX_Transactions_GuildId", IsUnique = false)]
[Index(nameof(GuildId), nameof(MessageId), nameof(MergedCount))]
[Index(nameof(GuildId), nameof(UserId))]
[Index(nameof(UserId))]
public class Transaction
{
    [Required]
    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string UserId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string MessageId { get; set; } = null!;

    [StringLength(100)]
    public string ReactionId { get; set; } = "";

    public int MergedCount { get; set; }
    public DateTime? MergeRangeFrom { get; set; }
    public DateTime? MergeRangeTo { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int Value { get; set; }

    [NotMapped]
    public bool IsReaction
        => !string.IsNullOrEmpty(ReactionId);
}
