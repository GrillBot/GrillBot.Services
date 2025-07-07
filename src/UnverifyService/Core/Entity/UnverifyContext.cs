using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace UnverifyService.Core.Entity;

public class UnverifyContext(DbContextOptions options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
}
