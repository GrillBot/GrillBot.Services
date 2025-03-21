using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core.Entity;
using RubbergodService.Models;

namespace RubbergodService.Actions.Karma;

public class GetKarmaPageAction(ICounterManager counterManager, RubbergodServiceContext dbContext) : ApiAction<RubbergodServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var parameters = (PaginatedParams)Parameters[0]!;

        var query = DbContext.Karma.AsNoTracking()
            .OrderByDescending(o => o.KarmaValue);

        var data = await ContextHelper.ReadEntitiesWithPaginationAsync(query, parameters);

        var counter = 0;
        var result = await PaginatedResponse<UserKarma>.CopyAndMapAsync(data, entity =>
        {
            counter++;
            return Task.FromResult(new UserKarma
            {
                Negative = entity.Negative,
                Positive = entity.Positive,
                Position = parameters.Skip() + counter,
                UserId = entity.MemberId,
                Value = entity.KarmaValue
            });
        });

        return ApiResult.Ok(result);
    }
}
