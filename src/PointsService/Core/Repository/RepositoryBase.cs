using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public abstract class RepositoryBase
{
    protected PointsServiceContext Context { get; }

    protected RepositoryBase(PointsServiceContext context)
    {
        Context = context;
    }
}
