using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class TransactionRepository : RepositoryBase
{
    public TransactionRepository(PointsServiceContext context) : base(context)
    {
    }
}
