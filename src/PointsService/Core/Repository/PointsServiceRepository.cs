using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class PointsServiceRepository : RepositoryBase<PointsServiceContext>
{
    public UserRepository User { get; }
    public TransactionRepository Transaction { get; }

    public PointsServiceRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
        User = new UserRepository(context, counterManager);
        Transaction = new TransactionRepository(context, counterManager);
    }
}
