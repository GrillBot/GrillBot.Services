using Microsoft.EntityFrameworkCore;

namespace PointsService.Core.Entity;

public class PointsServiceContext : DbContext
{
    public PointsServiceContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Channel>(builder => builder.HasKey(o => new { o.Id, o.GuildId }));
        modelBuilder.Entity<User>(builder => builder.HasKey(o => new { o.Id, o.GuildId }));
        modelBuilder.Entity<Transaction>(builder => builder.HasKey(o => new { o.GuildId, o.UserId, o.MessageId, o.ReactionId }));
    }

    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
}
