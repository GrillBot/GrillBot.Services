using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using MessageService.Core.Entity;
using MessageService.Models.Request.AutoReply;
using Response = MessageService.Models.Response.AutoReply;

namespace MessageService.Actions.AutoReply;

public class UpdateAutoReplyDefinitionAction(
    ICounterManager counterManager,
    MessageContext dbContext
) : ApiAction<MessageContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var id = GetParameter<Guid>(0);
        var request = GetParameter<AutoReplyDefinitionRequest>(0);

        var query = DbContext.AutoReplyDefinitions.Where(o => !o.IsDeleted && o.Id == id);
        var definition = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);

        if (definition is null)
            return ApiResult.NotFound();

        definition.Template = request.Template;
        definition.Reply = request.Reply;
        definition.IsCaseSensitive = request.IsCaseSensitive;
        definition.IsDisabled = request.IsDisabled;
        await ContextHelper.SaveChangesAsync();

        var result = new Response.AutoReplyDefinition(
            definition.Id,
            definition.Template,
            definition.Reply,
            false,
            definition.IsDisabled,
            definition.IsCaseSensitive
        );

        return ApiResult.Ok(result);
    }
}
