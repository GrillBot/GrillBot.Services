using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models.Users;

namespace PointsService.Actions.Users;

public class GetUserListAction : ApiAction
{
    public GetUserListAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (UserListRequest)Parameters[0]!;
        var query = CreateQuery(request);

        var result = await ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination);
        return ApiResult.Ok(result);
    }

    private IQueryable<UserListItem> CreateQuery(UserListRequest request)
    {
        var query = DbContext.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.GuildId == request.GuildId);

        return query
            .OrderByDescending(o => o.ActivePoints).ThenByDescending(o => o.ExpiredPoints).ThenByDescending(o => o.MergedPoints)
            .Select(o => new UserListItem
            {
                GuildId = o.GuildId,
                ActivePoints = o.ActivePoints,
                ExpiredPoints = o.ExpiredPoints,
                MergedPoints = o.MergedPoints,
                PointsDeactivated = o.PointsDisabled,
                UserId = o.Id
            });
    }
}
