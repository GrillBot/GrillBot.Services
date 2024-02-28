using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Models.Pagination;
using RubbergodService.Core.Models;
using RubbergodService.Core.Repository;

namespace RubbergodService.Actions.Karma;

public class GetKarmaPageAction : ApiActionBase
{
    private RubbergodServiceRepository Repository { get; }

    public GetKarmaPageAction(RubbergodServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var parameters = (PaginatedParams)Parameters[0]!;
        var karma = await Repository.Karma.GetKarmaPageAsync(parameters);

        var counter = 0;
        var result = await PaginatedResponse<UserKarma>.CopyAndMapAsync(karma, entity =>
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
