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
        modelBuilder.Entity<AuditLogDateStatistic>(builder => builder.HasKey(o => o.Date));
        modelBuilder.Entity<InteractionDateCountStatistic>(builder => builder.HasKey(o => o.Date));

        modelBuilder.Entity<ApiUserActionStatistic>(builder => builder.HasKey(o => new { o.Action, o.ApiGroup, o.IsPublic, o.UserId }));
        modelBuilder.Entity<InteractionUserActionStatistic>(builder => builder.HasKey(o => new { o.Action, o.UserId }));
    }

    public DbSet<ApiDateCountStatistic> DateCountStatistics => Set<ApiDateCountStatistic>();
    public DbSet<ApiRequestStat> RequestStats => Set<ApiRequestStat>();
    public DbSet<DailyAvgTimes> DailyAvgTimes => Set<DailyAvgTimes>();
    public DbSet<ApiUserActionStatistic> ApiUserActionStatistics => Set<ApiUserActionStatistic>();
    public DbSet<InteractionUserActionStatistic> InteractionUserActionStatistics => Set<InteractionUserActionStatistic>();
    public DbSet<AuditLogTypeStatistic> TypeStatistics => Set<AuditLogTypeStatistic>();
    public DbSet<AuditLogDateStatistic> DateStatistics => Set<AuditLogDateStatistic>();
    public DbSet<FileExtensionStatistic> FileExtensionStatistics => Set<FileExtensionStatistic>();
    public DbSet<InteractionStatistic> InteractionStatistics => Set<InteractionStatistic>();
    public DbSet<JobInfo> JobInfos => Set<JobInfo>();
    public DbSet<DatabaseStatistic> DatabaseStatistics => Set<DatabaseStatistic>();
    public DbSet<InteractionDateCountStatistic> InteractionDateCountStatistics => Set<InteractionDateCountStatistic>();
}
