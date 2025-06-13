using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Core.Entity;

public class UserManagementContext(DbContextOptions options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
    public DbSet<GuildUserNickname> Nicknames => Set<GuildUserNickname>();
    public DbSet<GuildUser> GuildUsers => Set<GuildUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuildUser>(builder =>
            builder.HasMany(o => o.Nicknames).WithOne(o => o.GuildUser).HasForeignKey(o => new { o.GuildId, o.UserId })
        );
    }
}
