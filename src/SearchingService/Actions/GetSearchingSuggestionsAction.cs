using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SearchingService.Core.Entity;
using SearchingService.Models.Response;
using SearchingService.Options;

namespace SearchingService.Actions;

public class GetSearchingSuggestionsAction : ApiAction<SearchingServiceContext>
{
    private readonly AppOptions _options;

    public GetSearchingSuggestionsAction(ICounterManager counterManager, SearchingServiceContext dbContext,
        IOptions<AppOptions> options) : base(counterManager, dbContext)
    {
        _options = options.Value;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<string>(0);
        var channelId = GetParameter<string>(1);
        var executingUserId = CurrentUser.Id ?? "0";
        var executingUser = (await ContextHelper.ReadFirstOrDefaultEntityAsync<User>(o => o.GuildId == guildId && o.UserId == executingUserId)) ?? new() { UserId = executingUserId };
        var result = await ReadItemsAsync(guildId, channelId, executingUser);

        return ApiResult.Ok(result);
    }

    private Task<List<SearchSuggestion>> ReadItemsAsync(string guildId, string channelId, User executingUser)
    {
        var query = DbContext.Items
            .Where(o => o.GuildId == guildId && o.ChannelId == channelId && o.ValidTo >= DateTime.UtcNow && !o.IsDeleted)
            .OrderBy(o => o.Id)
            .AsNoTracking();

        if (!executingUser.IsAdministrator)
            query = query.Where(o => o.UserId == executingUser.UserId);

        query = query.Take(_options.SuggestionsCount);

        var dataQuery = query.Select(o => new SearchSuggestion(o.Id, o.UserId, o.Content.Substring(0, 20)));
        return ContextHelper.ReadEntitiesAsync(dataQuery);
    }
}
