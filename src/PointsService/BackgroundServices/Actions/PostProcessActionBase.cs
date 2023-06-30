using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.Actions;

public abstract class PostProcessActionBase
{
    protected PointsServiceRepository Repository { get; }

    protected PostProcessActionBase(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public abstract Task ProcessAsync(User user);
}
