using Microsoft.EntityFrameworkCore;

namespace UserMeasuresService.Core.Entity;

public class UserMeasuresContext : DbContext
{
    public UserMeasuresContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UnverifyItem> Unverifies => Set<UnverifyItem>();
    public DbSet<MemberWarningItem> MemberWarnings => Set<MemberWarningItem>();
    public DbSet<TimeoutItem> Timeouts => Set<TimeoutItem>();
}
