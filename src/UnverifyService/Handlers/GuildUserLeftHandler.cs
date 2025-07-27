using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Events;

namespace UnverifyService.Handlers;

public class GuildUserLeftHandler(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<GuildUserLeftMessage, UnverifyContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        GuildUserLeftMessage message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var activeUnverifyQuery = DbContext.ActiveUnverifies.Where(o => o.LogItem.GuildId == message.GuildId && o.LogItem.ToUserId == message.UserId);
            var activeUnverify = await ContextHelper.ReadFirstOrDefaultEntityAsync(activeUnverifyQuery, cancellationToken);

            if (activeUnverify is null)
                return RabbitConsumptionResult.Success;

            DbContext.Remove(activeUnverify);
            await ContextHelper.SaveChangesAsync(cancellationToken);
            return RabbitConsumptionResult.Success;
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
                await RecalculateMetricsAsync(cancellationToken);
        }
    }

    private Task RecalculateMetricsAsync(CancellationToken cancellationToken = default)
        => Publisher.PublishAsync(new RecalculateMetricsMessage(), cancellationToken: cancellationToken);
}
