using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace RubbergodService.Core.Entity;

public class RubbergodServiceContext(DbContextOptions options, DatabaseTelemetryCollector connector) : GrillBotServiceDbContext(options, connector)
{
    public DbSet<Karma> Karma => Set<Karma>();
}
