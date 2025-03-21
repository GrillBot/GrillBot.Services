using Microsoft.EntityFrameworkCore;

namespace RemindService.Core.Entity;

public class RemindServiceContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<RemindMessage> RemindMessages => Set<RemindMessage>();
}
