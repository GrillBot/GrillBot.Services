using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using MessageService.Core.Entity;
using MessageService.Models.Request.AutoReply;
using Microsoft.EntityFrameworkCore;

using Response = MessageService.Models.Response.AutoReply;

namespace MessageService.Actions.AutoReply;

public class GetAutoReplyDefinitionListAction(
    ICounterManager counterManager,
    MessageContext dbContext
) : ApiAction<MessageContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<AutoReplyDefinitionListRequest>(0);

        var query = CreateQuery(request).Select(o => new Response.AutoReplyDefinition(
            o.Id,
            o.Template,
            o.Reply,
            o.IsDeleted,
            o.IsDisabled,
            o.IsCaseSensitive
        ));

        var result = await ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination);
        return ApiResult.Ok(result);
    }

    private IQueryable<AutoReplyDefinition> CreateQuery(AutoReplyDefinitionListRequest request)
    {
        var query = DbContext.AutoReplyDefinitions.AsNoTracking()
            .Where(o => !o.IsDeleted);

        if (!string.IsNullOrEmpty(request.TemplateContains))
            query = query.Where(o => EF.Functions.ILike(o.Template, $"%{request.TemplateContains}%"));

        if (!string.IsNullOrEmpty(request.ReplyContains))
            query = query.Where(o => EF.Functions.ILike(o.Reply, $"%{request.ReplyContains}%"));

        if (request.HideDisabled)
            query = query.Where(o => !o.IsDisabled);

        return query.WithSorting([o => o.Template], request.Sort.Descending);
    }
}
