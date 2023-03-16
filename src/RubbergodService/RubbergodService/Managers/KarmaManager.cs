using GrillBot.Core.Models.Pagination;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Models;
using RubbergodService.Core.Repository;
using RubbergodService.MemberSynchronization;

namespace RubbergodService.Managers;

public class KarmaManager
{
    private RubbergodServiceRepository Repository { get; }
    private MemberSyncQueue MemberSyncQueue { get; }

    public KarmaManager(RubbergodServiceRepository repository, MemberSyncQueue memberSyncQueue)
    {
        Repository = repository;
        MemberSyncQueue = memberSyncQueue;
    }

    public async Task StoreKarmaAsync(List<Karma> items)
    {
        foreach (var memberId in items.Select(o => o.MemberId).Distinct())
            await MemberSyncQueue.AddToQueueAsync(memberId);

        foreach (var item in items)
        {
            var dbKarma = await Repository.Karma.FindKarmaByMemberIdAsync(item.MemberId);
            if (dbKarma == null)
                await Repository.AddAsync(item);
            else
                dbKarma.Update(item);
        }

        await Repository.CommitAsync();
    }

    public async Task<PaginatedResponse<UserKarma>> GetPageAsync(PaginatedParams parameters)
    {
        var karma = await Repository.Karma.GetKarmaPageAsync(parameters);

        var counter = 0;
        return await PaginatedResponse<UserKarma>.CopyAndMapAsync(karma, async entity =>
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
    }
}
