using Microsoft.EntityFrameworkCore;

namespace RubbergodService.Core.Entity;

public class RubbergodServiceContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Karma> Karma => Set<Karma>();
}
