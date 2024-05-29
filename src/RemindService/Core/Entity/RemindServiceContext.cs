using Microsoft.EntityFrameworkCore;

namespace RemindService.Core.Entity;

public class RemindServiceContext : DbContext
{
    public RemindServiceContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<RemindMessage> RemindMessages => Set<RemindMessage>();
}
