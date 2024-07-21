using Microsoft.EntityFrameworkCore;

namespace SearchingService.Core.Entity;

public class SearchingServiceContext : DbContext
{
    public SearchingServiceContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<SearchItem> Items => Set<SearchItem>();
    public DbSet<User> Users => Set<User>();
}
