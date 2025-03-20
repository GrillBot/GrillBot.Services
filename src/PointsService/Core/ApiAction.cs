using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Core;

public abstract class ApiAction(
    ICounterManager counterManager,
    PointsServiceContext dbContext,
    IRabbitPublisher _publisher
) : GrillBot.Services.Common.Infrastructure.Api.ApiAction<PointsServiceContext>(counterManager, dbContext)
{
    protected IRabbitPublisher Publisher => _publisher;

    protected async Task<User?> FindUserAsync(string guildId, string userId)
    {
        var query = DbContext.Users.Where(o => o.GuildId == guildId && o.Id == userId).AsNoTracking();
        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }

    protected Task EnqueueUserForRecalculationAsync(string guildId, string userId)
        => Publisher.PublishAsync("Points", new UserRecalculationPayload(guildId, userId), "UserRecalculation");
}
