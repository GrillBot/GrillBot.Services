using System.ComponentModel.DataAnnotations;

namespace RubbergodService.Core.Entity;

public class PinCacheItem
{
    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [StringLength(32)]
    public string ChannelId { get; set; } = null!;

    [StringLength(255)]
    public string Filename { get; set; } = null!;
    
    public DateTime CreatedAtUtc { get; set; }

    public byte[] Data { get; set; } = null!;
}
