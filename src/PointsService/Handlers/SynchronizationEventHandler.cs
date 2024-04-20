using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Channels;
using PointsService.Models.Events;
using PointsService.Models.Users;

namespace PointsService.Handlers;

public class SynchronizationEventHandler : BasePointsEvent<SynchronizationPayload>
{
    public SynchronizationEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(SynchronizationPayload payload, Dictionary<string, string> headers)
    {
        foreach (var userInfo in payload.Users)
            await SynchronizeUserAsync(payload.GuildId, userInfo);

        foreach (var channelInfo in payload.Channels)
            await SynchronizeChannelAsync(payload.GuildId, channelInfo);

        await ContextHelper.SaveChagesAsync();
    }

    private async Task SynchronizeUserAsync(string guildId, UserSyncItem user)
    {
        var userQuery = DbContext.Users.Where(o => o.GuildId == guildId && o.Id == user.Id);
        var entity = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery);

        if (entity is null)
        {
            entity = new User
            {
                Id = user.Id,
                GuildId = guildId
            };

            await DbContext.AddAsync(entity);
        }

        if (user.PointsDisabled is not null)
            entity.PointsDisabled = user.PointsDisabled.Value;

        if (user.IsUser is not null)
            entity.IsUser = user.IsUser.Value;
    }

    private async Task SynchronizeChannelAsync(string guildId, ChannelSyncItem channel)
    {
        var channelQuery = DbContext.Channels.Where(o => o.GuildId == guildId && o.Id == channel.Id);
        var entity = await ContextHelper.ReadFirstOrDefaultEntityAsync(channelQuery);

        if (entity is null)
        {
            entity = new Channel
            {
                Id = channel.Id,
                GuildId = guildId
            };

            await DbContext.AddAsync(entity);
        }

        if (channel.IsDeleted is not null)
            entity.IsDeleted = channel.IsDeleted.Value;

        if (channel.PointsDisabled is not null)
            entity.PointsDisabled = channel.PointsDisabled.Value;
    }
}
