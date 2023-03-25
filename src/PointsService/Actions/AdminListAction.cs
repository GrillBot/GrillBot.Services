using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Models.Pagination;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class AdminListAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public AdminListAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (AdminListRequest)Parameters[0]!;
        var list = await Repository.Transaction.GetAdminListAsync(request);
        var result = await PaginatedResponse<TransactionItem>.CopyAndMapAsync(list, entity => Task.FromResult(new TransactionItem
        {
            CreatedAt = entity.CreatedAt,
            MergedCount = entity.MergedCount,
            Value = entity.Value,
            GuildId = entity.GuildId,
            IsReaction = !string.IsNullOrEmpty(entity.ReactionId),
            MergedFrom = entity.MergeRangeFrom,
            MergedTo = entity.MergeRangeTo,
            MessageId = entity.MessageId,
            UserId = entity.UserId
        }));

        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
