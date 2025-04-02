using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.Infrastructure.Api;
using InviteService.Core.Entity;
using InviteService.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace InviteService.Actions;

public class GetInviteUsesAction(
    ICounterManager counterManager,
    InviteContext dbContext
) : ApiAction<InviteContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<InviteUseListRequest>(0);

        var query = DbContext.InviteUses.AsNoTracking()
            .Where(o => o.GuildId == request.GuildId && o.Code == request.Code);

        if (request.UsedFrom is not null)
            query = query.Where(o => o.UsedAt >= request.UsedFrom.Value);

        if (request.UsedTo is not null)
            query = query.Where(o => o.UsedAt <= request.UsedTo.Value);

        query = request.Sort.Descending ?
            query.OrderByDescending(o => o.UsedAt) :
            query.OrderBy(o => o.UsedAt);

        var dataQuery = query.Select(o => new Models.Response.InviteUse(o.UserId, o.UsedAt));
        var result = await PaginatedResponse<Models.Response.InviteUse>.CreateWithEntityAsync(dataQuery, request.Pagination);

        return ApiResult.Ok(result);
    }
}
