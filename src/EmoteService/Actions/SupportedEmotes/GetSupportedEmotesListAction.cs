using EmoteService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.SupportedEmotes;

public class GetSupportedEmotesListAction : ApiAction<EmoteServiceContext>
{
    public GetSupportedEmotesListAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var definitionsQuery = DbContext.EmoteDefinitions.AsNoTracking();
        var definitions = await ContextHelper.ReadEntitiesAsync(definitionsQuery);
        var emotes = definitions.ConvertAll(d => d.ToString());

        return ApiResult.Ok(emotes);
    }
}
