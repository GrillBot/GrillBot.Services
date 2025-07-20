using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Redis.Extensions;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using InviteService.Core.Entity;
using InviteService.Models.Cache;
using InviteService.Models.Events;
using Microsoft.Extensions.Caching.Distributed;

namespace InviteService.Handlers;

public class InviteCreatedEventHandler(
    IServiceProvider serviceProvider,
    IDistributedCache _cache
) : BaseEventHandlerWithDb<InviteCreatedPayload, InviteContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        InviteCreatedPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var key = $"InviteMetadata-{message.GuildId}-{message.Code}";
        var metadata = new InviteMetadata(message.Code, message.Uses, message.CreatorId, message.CreatedAt);

        var invite = await _cache.GetAsync(key, cancellationToken);
        if (invite is not null)
            await _cache.RemoveAsync(key, cancellationToken);
        await _cache.SetAsync(key, metadata, null, cancellationToken);

        return RabbitConsumptionResult.Success;
    }
}
