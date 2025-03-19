using Microsoft.EntityFrameworkCore;

namespace EmoteService.Core.Entity;

public class EmoteServiceContext(DbContextOptions<EmoteServiceContext> options) : DbContext(options)
{
    public DbSet<EmoteDefinition> EmoteDefinitions => Set<EmoteDefinition>();
    public DbSet<EmoteUserStatItem> EmoteUserStatItems => Set<EmoteUserStatItem>();
}
