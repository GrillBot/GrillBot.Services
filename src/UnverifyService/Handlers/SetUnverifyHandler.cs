using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Events;

namespace UnverifyService.Handlers;

public class SetUnverifyHandler(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<SetUnverifyMessage, UnverifyContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        SetUnverifyMessage message,
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

        return RabbitConsumptionResult.Success;
    }
}
