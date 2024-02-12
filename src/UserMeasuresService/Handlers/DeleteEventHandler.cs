using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class DeleteEventHandler : BaseEventHandlerWithDb<DeletePayload>
{
    public override string QueueName => DeletePayload.QueueName;

    public DeleteEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext) : base(loggerFactory, dbContext)
    {
    }

    protected override async Task HandleInternalAsync(DeletePayload payload)
    {
        await DeleteWarningAsync(payload.Id);
    }

    private async Task DeleteWarningAsync(Guid id)
    {
        var item = await DbContext.MemberWarnings.FirstOrDefaultAsync(o => o.Id == id);
        if (item is null)
            return;

        DbContext.Remove(item);
        await DbContext.SaveChangesAsync();
    }
}
