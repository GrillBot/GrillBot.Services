using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class UserRepository : RepositoryBase
{
    public UserRepository(PointsServiceContext context) : base(context)
    {
    }
}
