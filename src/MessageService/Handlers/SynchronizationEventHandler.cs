using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using MessageService.Core.Entity;
using MessageService.Models.Events;
using MessageService.Models.Events.Channels;

namespace MessageService.Handlers;

public class SynchronizationEventHandler(IServiceProvider serviceProvider) : BaseEventHandlerWithDb<SynchronizationPayload, MessageContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(SynchronizationPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        foreach (var channel in message.Channels)
            await SynchronizeChannelAsync(channel);

        await ContextHelper.SaveChangesAsync();
        return RabbitConsumptionResult.Success;
    }

    private async Task SynchronizeChannelAsync(ChannelSynchronizationItem channel)
    {
        var channelQuery = DbContext.GuildChannels.Where(o => o.ChannelId == channel.ChannelId && o.GuildId == channel.GuildId);
        var entity = await ContextHelper.ReadFirstOrDefaultEntityAsync(channelQuery);

        if (entity is null)
        {
            entity = new GuildChannel
            {
                ChannelId = channel.ChannelId,
                GuildId = channel.GuildId
            };

            await DbContext.AddAsync(entity);
        }

        entity.IsDeleted = channel.IsDeleted ?? entity.IsDeleted;
        entity.IsPointsDisabled = channel.IsPointsDisabled ?? entity.IsPointsDisabled;
        entity.IsAutoReplyDisabled = channel.IsAutoReplyDisabled ?? entity.IsAutoReplyDisabled;
    }
}
