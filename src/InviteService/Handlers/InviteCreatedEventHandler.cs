using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Redis.Extensions;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using InviteService.Core.Entity;
using InviteService.Models.Cache;
using InviteService.Models.Events;
using Microsoft.Extensions.Caching.Distributed;

namespace InviteService.Handlers;

public class InviteCreatedEventHandler(
    ILoggerFactory loggerFactory,
    InviteContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher,
    IDistributedCache _cache
) : BaseEventHandlerWithDb<InviteCreatedPayload, InviteContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(InviteCreatedPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var key = $"InviteMetadata-{message.GuildId}-{message.Code}";
        var metadata = new InviteMetadata(message.Code, message.Uses, message.CreatorId, message.CreatedAt);

        var invite = await _cache.GetAsync(key);
        if (invite is not null)
            await _cache.RemoveAsync(key);
        await _cache.SetAsync(key, metadata, null);

        return RabbitConsumptionResult.Success;
    }
}
