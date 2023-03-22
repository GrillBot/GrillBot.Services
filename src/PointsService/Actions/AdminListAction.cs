using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Models.Pagination;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class AdminListAction : IApiAction
{
    private PointsServiceRepository Repository { get; }

    public AdminListAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public async Task<ApiResult> ProcessAsync(object?[] parameters)
    {
        var request = (AdminListRequest)parameters[0]!;
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
