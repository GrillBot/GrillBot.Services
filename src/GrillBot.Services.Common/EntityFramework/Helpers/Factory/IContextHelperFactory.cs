using Microsoft.EntityFrameworkCore;

namespace GrillBot.Services.Common.EntityFramework.Helpers.Factory;

public interface IContextHelperFactory<TDbContext> where TDbContext : DbContext
{
    ContextHelper<TDbContext> Create(string counterKey);
}
