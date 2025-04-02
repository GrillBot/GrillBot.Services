using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.Infrastructure.Api;
using InviteService.Core.Entity;
using InviteService.Models.Request;
using InviteService.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace InviteService.Actions;

public class GetUserInviteUsesAction(
    ICounterManager counterManager,
    InviteContext dbContext
) : ApiAction<InviteContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<UserInviteUseListRequest>(0);

        var query = DbContext.InviteUses.AsNoTracking()
            .Where(o => o.UserId == request.UserId)
            .GroupBy(o => new { o.GuildId, o.Code });

        query = request.Sort.Descending ?
            query.OrderByDescending(o => o.Max(x => x.UsedAt)) :
            query.OrderBy(o => o.Max(x => x.UsedAt));

        var dataQuery = query.Select(o => new UserInviteUse(o.Key.GuildId, o.Key.Code, o.Max(x => x.UsedAt)));
        var result = await PaginatedResponse<UserInviteUse>.CreateWithEntityAsync(dataQuery, request.Pagination);

        return ApiResult.Ok(result);
    }
}
