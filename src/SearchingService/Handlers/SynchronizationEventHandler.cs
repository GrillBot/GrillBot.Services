using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using SearchingService.Core.Entity;
using SearchingService.Models.Events;
using SearchingService.Models.Events.Users;

namespace SearchingService.Handlers;

public class SynchronizationEventHandler : BaseEventHandlerWithDb<SynchronizationPayload, SearchingServiceContext>
{
    public SynchronizationEventHandler(ILoggerFactory loggerFactory, SearchingServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(SynchronizationPayload payload, Dictionary<string, string> headers)
    {
        foreach (var user in payload.Users)
            await SynchonizeUserAsync(user);

        await ContextHelper.SaveChagesAsync();
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
