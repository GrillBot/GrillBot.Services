using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.Actions;

public abstract class PostProcessActionBase
{
    protected PointsServiceRepository Repository { get; }
    protected PointsServiceContext Context { get; }

    protected PostProcessActionBase(IServiceProvider serviceProvider)
    {
        Repository = serviceProvider.GetRequiredService<PointsServiceRepository>();
        Context = serviceProvider.GetRequiredService<PointsServiceContext>();
    }

    public abstract Task ProcessAsync(User user);
}
