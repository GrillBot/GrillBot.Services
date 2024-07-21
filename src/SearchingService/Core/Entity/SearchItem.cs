using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchingService.Core.Entity;

public class SearchItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [StringLength(30)]
    public string UserId { get; set; } = null!;

    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    [StringLength(30)]
    public string ChannelId { get; set; } = null!;

    [StringLength(EmbedFieldBuilder.MaxFieldValueLength)]
    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime ValidTo { get; set; }
}
