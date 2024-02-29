using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class DeleteEventHandler : BaseMeasuresHandler<DeletePayload>
{
    public DeleteEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(DeletePayload payload)
    {
        await DeleteWarningAsync(payload.Id);
    }

    private async Task DeleteWarningAsync(Guid id)
    {
        var item = await ReadWarningAsync(id);
        if (item is null)
            return;

        using (CreateCounter("Database"))
        {
            DbContext.Remove(item);
            await DbContext.SaveChangesAsync();
        }
    }

    private async Task<MemberWarningItem?> ReadWarningAsync(Guid id)
    {
        using (CreateCounter("Database"))
            return await DbContext.MemberWarnings.FirstOrDefaultAsync(o => o.Id == id);
    }
}
