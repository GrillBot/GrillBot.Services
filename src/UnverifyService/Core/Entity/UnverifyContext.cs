using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity.Logs;

namespace UnverifyService.Core.Entity;

public class UnverifyContext(DbContextOptions options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UnverifyLogItem> LogItems => Set<UnverifyLogItem>();
    public DbSet<ActiveUnverify> ActiveUnverifies => Set<ActiveUnverify>();
    public DbSet<SelfUnverifyKeepable> SelfUnverifyKeepables => Set<SelfUnverifyKeepable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UnverifyLogItem>(b =>
        {
            b.WithSchema("logs");

            b.HasOne(o => o.SetOperation).WithOne(o => o.LogItem).HasForeignKey<UnverifyLogSetOperation>(o => o.LogItemId);
            b.HasOne(o => o.RemoveOperation).WithOne(o => o.LogItem).HasForeignKey<UnverifyLogRemoveOperation>(o => o.LogItemId);
            b.HasOne(o => o.UpdateOperation).WithOne(o => o.LogItem).HasForeignKey<UnverifyLogUpdateOperation>(o => o.LogItemId);
            b.HasOne(o => o.ParentLogItem).WithMany(o => o.ChildLogItems).HasForeignKey(o => o.ParentLogItemId);
        });

        modelBuilder.Entity<UnverifyLogRemoveChannel>(b =>
        {
            b.HasKey(o => new { o.LogItemId, o.ChannelId });
            b.WithSchema("logs");
        });

        modelBuilder.Entity<UnverifyLogRemoveOperation>(b =>
        {
            b.WithSchema("logs");

            b.HasMany(o => o.Roles).WithOne(o => o.Operation).HasForeignKey(o => o.LogItemId);
            b.HasMany(o => o.Channels).WithOne(o => o.Operation).HasForeignKey(o => o.LogItemId);
        });

        modelBuilder.Entity<UnverifyLogRemoveRole>(b =>
        {
            b.HasKey(o => new { o.LogItemId, o.RoleId });
            b.WithSchema("logs");
        });

        modelBuilder.Entity<UnverifyLogSetChannel>(b =>
        {
            b.HasKey(o => new { o.LogItemId, o.ChannelId });
            b.WithSchema("logs");
        });

        modelBuilder.Entity<UnverifyLogSetOperation>(b =>
        {
            b.WithSchema("logs");

            b.HasMany(o => o.Roles).WithOne(o => o.Operation).HasForeignKey(o => o.LogItemId);
            b.HasMany(o => o.Channels).WithOne(o => o.Operation).HasForeignKey(o => o.LogItemId);
        });

        modelBuilder.Entity<UnverifyLogSetRole>(b =>
        {
            b.HasKey(o => new { o.LogItemId, o.RoleId });
            b.WithSchema("logs");
        });

        modelBuilder.Entity<UnverifyLogUpdateOperation>(b => b.WithSchema("logs"));
        modelBuilder.Entity<ActiveUnverify>(b => b.HasOne(o => o.LogItem).WithOne().HasForeignKey<ActiveUnverify>(o => o.LogSetId));
        modelBuilder.Entity<SelfUnverifyKeepable>(b => b.HasKey(o => new { o.Group, o.Name }));
    }
}
