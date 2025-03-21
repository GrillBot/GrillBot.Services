using Microsoft.EntityFrameworkCore;

namespace SearchingService.Core.Entity;

public class SearchingServiceContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SearchItem> Items => Set<SearchItem>();
    public DbSet<User> Users => Set<User>();
}
