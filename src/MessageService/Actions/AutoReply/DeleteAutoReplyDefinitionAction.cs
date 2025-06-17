using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using MessageService.Core.Entity;

namespace MessageService.Actions.AutoReply;

public class DeleteAutoReplyDefinitionAction(
    ICounterManager counterManager,
    MessageContext dbContext
) : ApiAction<MessageContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var id = GetParameter<Guid>(0);

        var query = DbContext.AutoReplyDefinitions.Where(o => o.Id == id);
        var definition = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);

        if (definition is null)
            return ApiResult.NotFound();

        definition.IsDeleted = true;
        await ContextHelper.SaveChangesAsync();
        return ApiResult.Ok();
    }
}
