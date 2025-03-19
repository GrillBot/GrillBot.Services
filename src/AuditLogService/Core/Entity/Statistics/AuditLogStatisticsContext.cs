using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Entity.Statistics;

public class AuditLogStatisticsContext(DbContextOptions<AuditLogStatisticsContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("statistics");

        modelBuilder.Entity<ApiUserActionStatistic>(builder => builder.HasKey(o => new { o.Action, o.ApiGroup, o.IsPublic, o.UserId }));
        modelBuilder.Entity<InteractionUserActionStatistic>(builder => builder.HasKey(o => new { o.Action, o.UserId }));
    }

    public DbSet<ApiRequestStat> RequestStats => Set<ApiRequestStat>();
    public DbSet<DailyAvgTimes> DailyAvgTimes => Set<DailyAvgTimes>();
    public DbSet<ApiUserActionStatistic> ApiUserActionStatistics => Set<ApiUserActionStatistic>();
    public DbSet<InteractionUserActionStatistic> InteractionUserActionStatistics => Set<InteractionUserActionStatistic>();
    public DbSet<InteractionStatistic> InteractionStatistics => Set<InteractionStatistic>();
    public DbSet<JobInfo> JobInfos => Set<JobInfo>();
    public DbSet<DatabaseStatistic> DatabaseStatistics => Set<DatabaseStatistic>();
}
