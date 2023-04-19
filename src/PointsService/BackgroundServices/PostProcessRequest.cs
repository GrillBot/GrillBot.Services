using PointsService.Core.Entity;

namespace PointsService.BackgroundServices;

public class PostProcessRequest
{
    public Transaction Transaction { get; }
    public bool IsDelete { get; }

    public PostProcessRequest(Transaction transaction, bool isDelete)
    {
        Transaction = transaction;
        IsDelete = isDelete;
    }
}
