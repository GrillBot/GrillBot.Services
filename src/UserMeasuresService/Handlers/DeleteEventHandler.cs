using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class DeleteEventHandler : BaseEventHandlerWithDb<DeletePayload>
{
    public override string QueueName => new DeletePayload().QueueName;

    public DeleteEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager)
        : base(loggerFactory, dbContext, counterManager)
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
