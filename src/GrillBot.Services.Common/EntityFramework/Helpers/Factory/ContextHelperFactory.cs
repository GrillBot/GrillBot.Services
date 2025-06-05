using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.EntityFramework.Helpers.Factory;

public class ContextHelperFactory<TDbContext>(IServiceProvider _serviceProvider) : IContextHelperFactory<TDbContext>
    where TDbContext : DbContext
{
    public ContextHelper<TDbContext> Create(string counterKey)
    {
        var counterManager = _serviceProvider.GetRequiredService<ICounterManager>();
        var dbContext = _serviceProvider.GetRequiredService<TDbContext>();

        return new ContextHelper<TDbContext>(counterManager, dbContext, counterKey);
    }
}
