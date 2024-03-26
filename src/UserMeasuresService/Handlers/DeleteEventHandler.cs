using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
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

        DbContext.Remove(item);
        await ContextHelper.SaveChagesAsync();
    }

    private async Task<MemberWarningItem?> ReadWarningAsync(Guid id)
        => await ContextHelper.ReadFirstOrDefaultEntityAsync(DbContext.MemberWarnings.Where(o => o.Id == id));
}
