using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Events;

namespace UnverifyService.Handlers;

public class SynchronizationHandler(IServiceProvider serviceProvider)
    : BaseEventHandlerWithDb<SynchronizationMessage, UnverifyContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        SynchronizationMessage message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var user in message.Users)
            await SynchronizeUserAsync(user, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);
        return RabbitConsumptionResult.Success;
    }

    private async Task SynchronizeUserAsync(UserSyncMessage syncItem, CancellationToken cancellationToken = default)
    {
        var userQuery = DbContext.Users.Where(o => o.Id == syncItem.UserId);
        var entity = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, cancellationToken);

        if (entity is null)
        {
            entity = new User
            {
                Id = syncItem.UserId,
            };

            await DbContext.AddAsync(entity, cancellationToken);
        }

        entity.IsBot = syncItem.IsBot;
        entity.Language = syncItem.UserLanguage;
    }
}
