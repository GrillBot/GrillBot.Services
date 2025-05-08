using Discord;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using SearchingService.Core.Entity;
using SearchingService.Models.Events;
using SearchingService.Models.Events.Users;

namespace SearchingService.Handlers;

public class SynchronizationEventHandler(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<SynchronizationPayload, SearchingServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(SynchronizationPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        foreach (var user in message.Users)
            await SynchonizeUserAsync(user);

        await ContextHelper.SaveChangesAsync();
        return RabbitConsumptionResult.Success;
    }

    private async Task SynchonizeUserAsync(UserSynchronizationItem user)
    {
        var entity = await ContextHelper.ReadFirstOrDefaultEntityAsync<User>(o => o.GuildId == user.GuildId && o.UserId == user.UserId);

        if (entity is null)
        {
            entity = new User
            {
                UserId = user.UserId,
                GuildId = user.GuildId
            };

            await DbContext.AddAsync(entity);
        }

        var permissions = new GuildPermissions((ulong)user.GuildPermissions);
        entity.HaveGuildAdmin = permissions.Administrator;
        entity.HaveManageMessages = permissions.ManageMessages;
        entity.IsSearchingAdmin = user.IsAdmin;
    }
}
