using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.Infrastructure.Api;
using InviteService.Core.Entity;
using InviteService.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace InviteService.Actions;

public class GetUsedInvitesAction(
    ICounterManager counterManager,
    InviteContext dbContext
) : ApiAction<InviteContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<InviteListRequest>(0);
        var query = DbContext.Invites.AsNoTracking()
            .Where(o => o.Uses.Count > 0);

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.GuildId == request.GuildId);

        if (!string.IsNullOrEmpty(request.CreatorId))
            query = query.Where(o => o.CreatorId == request.CreatorId);
        else if (request.OnlyWithoutCreator)
            query = query.Where(o => o.CreatorId == null);

        if (!string.IsNullOrEmpty(request.Code))
            query = query.Where(o => EF.Functions.ILike(o.Code, $"{request.Code.ToLower()}%"));

        if (request.CreatedFrom is not null)
            query = query.Where(o => o.CreatedAt >= request.CreatedFrom.Value);
        if (request.CreatedTo is not null)
            query = query.Where(o => o.CreatedAt <= request.CreatedTo.Value);

        query = request.Sort.OrderBy?.ToLower() switch
        {
            "created" => request.Sort.Descending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
            "uses" => request.Sort.Descending ? query.OrderByDescending(o => o.Uses.Count) : query.OrderBy(o => o.Uses.Count),
            _ => request.Sort.Descending ? query.OrderByDescending(o => o.Code) : query.OrderBy(o => o.Code)
        };

        var dataQuery = query.Select(o => new Models.Response.Invite(
            o.Code,
            o.GuildId,
            o.CreatorId,
            o.CreatedAt,
            o.Uses.Count
        ));

        var result = await PaginatedResponse<Models.Response.Invite>.CreateWithEntityAsync(dataQuery, request.Pagination);
        return ApiResult.Ok(result);
    }
}
