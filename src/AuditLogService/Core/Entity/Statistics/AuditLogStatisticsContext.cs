using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Entity.Statistics;

public class AuditLogStatisticsContext : DbContext
{
    public AuditLogStatisticsContext(DbContextOptions<AuditLogStatisticsContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("statistics");

        modelBuilder.Entity<ApiDateCountStatistic>(builder => builder.HasKey(o => new { o.ApiGroup, o.Date }));
        modelBuilder.Entity<ApiResultCountStatistic>(builder => builder.HasKey(o => new { o.ApiGroup, o.Result }));
    }

    public DbSet<ApiDateCountStatistic> DateCountStatistics => Set<ApiDateCountStatistic>();
    public DbSet<ApiResultCountStatistic> ResultCountStatistic => Set<ApiResultCountStatistic>();
    public DbSet<ApiRequestStat> RequestStats => Set<ApiRequestStat>();
    public DbSet<DailyAvgTimes> DailyAvgTimes => Set<DailyAvgTimes>();
}
