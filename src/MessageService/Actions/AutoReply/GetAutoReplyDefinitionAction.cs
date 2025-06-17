using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using MessageService.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Actions.AutoReply;

public class GetAutoReplyDefinitionAction(
    ICounterManager counterManager,
    MessageContext dbContext
) : ApiAction<MessageContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var id = GetParameter<Guid>(0);

        var query = DbContext.AutoReplyDefinitions.AsNoTracking()
            .Where(o => !o.IsDeleted && o.Id == id)
            .Select(o => new Models.Response.AutoReply.AutoReplyDefinition(
                o.Id,
                o.Template,
                o.Reply,
                o.IsDeleted,
                o.IsDisabled,
                o.IsCaseSensitive
            ));

        var result = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);

        if (result is null)
            return ApiResult.NotFound();
        return ApiResult.Ok(result);
    }
}
