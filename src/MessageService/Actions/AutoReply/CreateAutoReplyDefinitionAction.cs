using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using MessageService.Core.Entity;
using MessageService.Models.Request.AutoReply;
using Response = MessageService.Models.Response.AutoReply;

namespace MessageService.Actions.AutoReply;

public class CreateAutoReplyDefinitionAction(
    ICounterManager counterManager,
    MessageContext dbContext
) : ApiAction<MessageContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<AutoReplyDefinitionRequest>(0);

        var definition = new AutoReplyDefinition
        {
            Id = Guid.NewGuid(),
            IsCaseSensitive = request.IsCaseSensitive,
            IsDeleted = false,
            IsDisabled = request.IsDisabled,
            Reply = request.Reply,
            Template = request.Template
        };

        await DbContext.AddAsync(definition);
        await ContextHelper.SaveChangesAsync();

        var result = new Response.AutoReplyDefinition(
            definition.Id,
            definition.Template,
            definition.Reply,
            definition.IsDeleted,
            definition.IsDisabled,
            definition.IsCaseSensitive
        );

        return ApiResult.Ok(result);
    }
}
