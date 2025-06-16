using GrillBot.Services.Common.EntityFramework;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Core.Entity;

public class MessageContext(DbContextOptions options, DatabaseTelemetryCollector collector) : GrillBotServiceDbContext(options, collector)
{
    public DbSet<AutoReplyDefinition> AutoReplyDefinitions => Set<AutoReplyDefinition>();
    public DbSet<GuildChannel> GuildChannels => Set<GuildChannel>();
}
