using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace SearchingService.Core.Entity;

public class SearchingServiceContext(DbContextOptions options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
    public DbSet<SearchItem> Items => Set<SearchItem>();
    public DbSet<User> Users => Set<User>();
}
