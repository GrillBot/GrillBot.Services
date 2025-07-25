﻿using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class TimeoutEventHandler(
    IServiceProvider serviceProvider
) : BaseMeasuresHandler<TimeoutPayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        TimeoutPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await GetOrCreateEntityAsync(message.ExternalId);

        entity.CreatedAtUtc = message.CreatedAtUtc;
        entity.GuildId = message.GuildId;
        entity.ModeratorId = message.ModeratorId;
        entity.Reason = message.Reason;
        entity.UserId = message.TargetUserId;
        entity.ValidTo = message.ValidToUtc;

        await SaveEntityAsync(entity);
        return RabbitConsumptionResult.Success;
    }

    private async Task<TimeoutItem> GetOrCreateEntityAsync(long externalId)
    {
        var query = DbContext.Timeouts.Where(o => o.ExternalId == externalId);
        var item = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);

        return item ?? new TimeoutItem { ExternalId = externalId };
    }
}
