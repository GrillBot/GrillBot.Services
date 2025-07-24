using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UnverifyService.Core.Entity;

namespace UnverifyService.Handlers;

public abstract class UnverifyServiceBaseHandler<TPayload>(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<TPayload, UnverifyContext>(serviceProvider) where TPayload : class, IRabbitMessage, new()
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        TPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        if (!currentUser.IsLogged)
        {
            await NotifyUnauthorizedExecution(message, cancellationToken);
            return RabbitConsumptionResult.Reject;
        }

        return await ProcessHandlerAsync(message, currentUser, headers, cancellationToken);
    }

    protected abstract Task<RabbitConsumptionResult> ProcessHandlerAsync(TPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> handler, CancellationToken cancellationToken = default);
}
