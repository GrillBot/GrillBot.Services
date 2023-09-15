using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Users;

namespace PointsService.Actions.Users;

public class UserListAction : ApiActionBase
{
    private PointsServiceContext Context { get; }
    private ICounterManager CounterManager { get; }

    public UserListAction(PointsServiceContext context, ICounterManager counterManager)
    {
        Context = context;
        CounterManager = counterManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (UserListRequest)Parameters[0]!;
        var query = CreateQuery(request);
        var result = await ReadDataFromQueryAsync(query, request.Pagination);

        return ApiResult.FromSuccess(result);
    }

    private IQueryable<UserListItem> CreateQuery(UserListRequest request)
    {
        var query = Context.Users.AsNoTracking();

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

    private async Task<PaginatedResponse<UserListItem>> ReadDataFromQueryAsync(IQueryable<UserListItem> query, PaginatedParams pagination)
    {
        using (CounterManager.Create("Api.Database.ReadUsers"))
        {
            return await PaginatedResponse<UserListItem>.CreateWithEntityAsync(query, pagination);
        }
    }
}
