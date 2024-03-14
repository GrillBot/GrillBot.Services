using EmoteService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.SupportedEmotes;

public class GetSupportedEmotesListAction : ApiAction
{
    private readonly EmoteServiceContext _dbContext;

    public GetSupportedEmotesListAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager)
    {
        _dbContext = dbContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var definitions = await GetDefinitionsAsync(guildId);
        var emotes = definitions.ConvertAll(d => d.ToString());

        return ApiResult.Ok(emotes);
    }

    private async Task<List<EmoteDefinition>> GetDefinitionsAsync(string guildId)
    {
        using (CreateCounter("Database"))
            return await _dbContext.EmoteDefinitions.AsNoTracking().Where(o => o.GuildId == guildId).ToListAsync();
    }
}
