using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Entity;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class SynchronizationAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public SynchronizationAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (SynchronizationRequest)Parameters[0]!;

        await ProcessUsersAsync(request);
        await ProcessChannelsAsync(request);
        await Repository.CommitAsync();

        return ApiResult.Ok();
    }

    private async Task ProcessUsersAsync(SynchronizationRequest request)
    {
        foreach (var userInfo in request.Users)
        {
            var user = await Repository.User.FindUserAsync(request.GuildId, userInfo.Id);
            if (user is null)
            {
                user = new User
                {
                    Id = userInfo.Id,
                    GuildId = request.GuildId
                };

                await Repository.AddAsync(user);
            }

            user.IsUser = userInfo.IsUser;
            user.PointsDisabled = userInfo.PointsDisabled;
            user.PendingRecalculation = true;
        }
    }

    private async Task ProcessChannelsAsync(SynchronizationRequest request)
    {
        foreach (var channelInfo in request.Channels)
        {
            var channel = await Repository.Channel.FindChannelAsync(request.GuildId, channelInfo.Id);
            if (channel is null)
            {
                channel = new Channel
                {
                    Id = channelInfo.Id,
                    GuildId = request.GuildId
                };

                await Repository.AddAsync(channel);
            }

            channel.PointsDisabled = channelInfo.PointsDisabled;
            channel.IsDeleted = channelInfo.IsDeleted;
        }
    }
}
