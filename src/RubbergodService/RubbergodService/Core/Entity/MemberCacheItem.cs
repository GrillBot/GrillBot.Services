using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubbergodService.Core.Entity;

public class MemberCacheItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(32)]
    public string UserId { get; set; } = null!;

    [StringLength(32)]
    [MinLength(2)]
    [Required]
    public string Username { get; set; } = null!;

    [StringLength(4)]
    [Required]
    public string Discriminator { get; set; } = null!;

    [Required]
    public string AvatarUrl { get; set; } = null!;
}
