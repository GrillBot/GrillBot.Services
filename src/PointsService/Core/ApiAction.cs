using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Core;

public abstract class ApiAction : GrillBot.Services.Common.Infrastructure.Api.ApiAction
{
    protected PointsServiceContext DbContext { get; }
    protected IRabbitMQPublisher Publisher { get; }

    protected ApiAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher) : base(counterManager)
    {
        DbContext = dbContext;
        Publisher = publisher;
    }

    protected async Task<User?> FindUserAsync(string guildId, string userId)
    {
        using (CreateCounter("Database"))
            return await DbContext.Users.AsNoTracking().FirstOrDefaultAsync(o => o.GuildId == guildId && o.Id == userId);
    }

    protected Task EnqueueUserForRecalculationAsync(string guildId, string userId)
        => Publisher.PublishAsync(new UserRecalculationPayload(guildId, userId));
}
