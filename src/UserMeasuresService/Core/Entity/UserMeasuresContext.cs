using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace UserMeasuresService.Core.Entity;

public class UserMeasuresContext(DbContextOptions options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
    public DbSet<UnverifyItem> Unverifies => Set<UnverifyItem>();
    public DbSet<MemberWarningItem> MemberWarnings => Set<MemberWarningItem>();
    public DbSet<TimeoutItem> Timeouts => Set<TimeoutItem>();
}
