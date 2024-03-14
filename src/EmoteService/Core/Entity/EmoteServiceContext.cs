using Microsoft.EntityFrameworkCore;

namespace EmoteService.Core.Entity;

public class EmoteServiceContext : DbContext
{
    public EmoteServiceContext(DbContextOptions<EmoteServiceContext> options) : base(options)
    {
    }

    public DbSet<EmoteDefinition> EmoteDefinitions => Set<EmoteDefinition>();
    public DbSet<EmoteUserStatItem> EmoteUserStatItems => Set<EmoteUserStatItem>();
}
