using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Core;

public abstract class ApiAction : GrillBot.Services.Common.Infrastructure.Api.ApiAction<PointsServiceContext>
{
    protected IRabbitMQPublisher Publisher { get; }

    protected ApiAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher) : base(counterManager, dbContext)
    {
        Publisher = publisher;
    }

    protected async Task<User?> FindUserAsync(string guildId, string userId)
    {
        var query = DbContext.Users.Where(o => o.GuildId == guildId && o.Id == userId).AsNoTracking();
        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }

    protected Task EnqueueUserForRecalculationAsync(string guildId, string userId)
        => Publisher.PublishAsync(new UserRecalculationPayload(guildId, userId));
}
