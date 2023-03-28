using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Models.Pagination;
using RubbergodService.Core.Models;
using RubbergodService.Core.Repository;

namespace RubbergodService.Actions;

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
        var result = await PaginatedResponse<UserKarma>.CopyAndMapAsync(karma, async entity =>
        {
            var user = (await Repository.MemberCache.FindMemberByIdAsync(entity.MemberId))!;

            counter++;
            return new UserKarma
            {
                Negative = entity.Negative,
                Positive = entity.Positive,
                Position = parameters.Skip() + counter,
                User = new User
                {
                    Discriminator = user.Discriminator,
                    AvatarUrl = user.AvatarUrl,
                    Username = user.Username,
                    Id = user.UserId
                },
                Value = entity.KarmaValue
            };
        });

        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
