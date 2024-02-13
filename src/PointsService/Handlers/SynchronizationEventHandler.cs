using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models;
using PointsService.Models.Events;
using PointsService.Models.Users;

namespace PointsService.Handlers;

public class SynchronizationEventHandler : BaseEventWithDb<SynchronizationPayload>
{
    public override string QueueName => SynchronizationPayload.QueueName;

    public SynchronizationEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(SynchronizationPayload payload)
    {
        foreach (var userInfo in payload.Users)
            await SynchronizeUserAsync(payload.GuildId, userInfo);

        foreach (var channelInfo in payload.Channels)
            await SynchronizeChannelAsync(payload.GuildId, channelInfo);

        await DbContext.SaveChangesAsync();
    }

    private async Task SynchronizeUserAsync(string guildId, UserInfo userInfo)
    {
        var entity = await DbContext.Users.FirstOrDefaultAsync(o => o.GuildId == guildId && o.Id == userInfo.Id);
        if (entity is null)
        {
            entity = new User
            {
                Id = userInfo.Id,
                GuildId = guildId
            };

            await DbContext.AddAsync(entity);
        }

        entity.PointsDisabled = userInfo.PointsDisabled;
        entity.IsUser = userInfo.IsUser;
    }

    private async Task SynchronizeChannelAsync(string guildId, ChannelInfo channelInfo)
    {
        var entity = await DbContext.Channels.FirstOrDefaultAsync(o => o.GuildId == guildId && o.Id == channelInfo.Id);
        if (entity is null)
        {
            entity = new Channel
            {
                Id = channelInfo.Id,
                GuildId = guildId
            };

            await DbContext.AddAsync(entity);
        }

        entity.IsDeleted = channelInfo.IsDeleted;
        entity.PointsDisabled = channelInfo.PointsDisabled;
    }
}
