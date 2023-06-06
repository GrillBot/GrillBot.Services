using Microsoft.EntityFrameworkCore;

namespace RubbergodService.Core.Entity;

public class RubbergodServiceContext : DbContext
{
    public RubbergodServiceContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PinCacheItem>(builder => builder.HasKey(e => new { e.GuildId, e.ChannelId, e.Filename }));
    }

    public DbSet<Karma> Karma => Set<Karma>();
    public DbSet<MemberCacheItem> MemberCache => Set<MemberCacheItem>();
    public DbSet<PinCacheItem> PinCache => Set<PinCacheItem>();
}
