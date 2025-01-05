using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SearchingService.Core.Entity;

namespace SearchingService.Actions;

public class RemoveSearchingAction : ApiAction<SearchingServiceContext>
{
    public RemoveSearchingAction(ICounterManager counterManager, SearchingServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var id = GetParameter<long>(0);

        var search = await ContextHelper.ReadFirstOrDefaultEntityAsync<SearchItem>(o => o.Id == id && !o.IsDeleted);
        if (search is null)
            return ApiResult.NotFound();

        var modelState = await CheckSearchStatusAsync(search);
        if (!modelState.IsValid)
            return ApiResult.BadRequest(new ValidationProblemDetails(modelState));

        search.IsDeleted = true;
        await ContextHelper.SaveChagesAsync();

        return ApiResult.Ok();
    }

    private async Task<ModelStateDictionary> CheckSearchStatusAsync(SearchItem item)
    {
        var modelState = new ModelStateDictionary();

        var guildId = Array.Find(CurrentUser.Permissions, o => o.StartsWith("GuildId:"))?.Replace("GuildId:", "");
        var query = DbContext.Users.Where(o => o.UserId == CurrentUser.Id);

        if (!CurrentUser.Permissions.Contains("Searching(Admin)"))
            query = query.Where(o => o.GuildId == guildId);

        var currentUsers = await ContextHelper.ReadEntitiesAsync(query);

        if (currentUsers.Count == 0)
        {
            modelState.AddModelError("User", "SearchingModule/RemoveSearch/UnknownExecutor");
            return modelState;
        }

        var isSomewhereAdmin = currentUsers.Exists(o => o.IsAdministrator);
        if (item.UserId != CurrentUser.Id && !isSomewhereAdmin)
            modelState.AddModelError("User", "SearchingModule/RemoveSearch/InsufficientPermissions");

        return modelState;
    }
}
