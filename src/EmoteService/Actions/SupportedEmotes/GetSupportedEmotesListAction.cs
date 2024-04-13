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
        var guildId = GetOptionalParameter<string>(0);
        var definitionsQuery = DbContext.EmoteDefinitions.AsNoTracking();
        if (!string.IsNullOrEmpty(guildId))
            definitionsQuery = definitionsQuery.Where(o => o.GuildId == guildId);

        var definitions = await ContextHelper.ReadEntitiesAsync(definitionsQuery);
        var emotes = definitions.ConvertAll(d => new Models.Response.EmoteDefinition
        {
            FullId = d.ToString(),
            GuildId = d.GuildId
        });

        return ApiResult.Ok(emotes);
    }
}
