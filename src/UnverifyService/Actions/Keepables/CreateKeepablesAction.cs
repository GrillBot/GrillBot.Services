using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Request.Keepables;

namespace UnverifyService.Actions.Keepables;

public class CreateKeepablesAction(UnverifyContext dbContext, ICounterManager counterManager) : ApiAction<UnverifyContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var requests = GetParameter<List<CreateKeepableRequest>>(0);

        var validationResult = await ValidateRequestsAsync(requests);
        if (validationResult is not null)
            return ApiResult.BadRequest(validationResult);

        var entities = requests.Select(o => new SelfUnverifyKeepable
        {
            Group = o.Group,
            Name = o.Name,
            CreatedAtUtc = DateTime.UtcNow
        });

        await DbContext.AddRangeAsync(entities, CancellationToken);
        await ContextHelper.SaveChangesAsync(CancellationToken);

        return ApiResult.Ok();
    }

    private async Task<ValidationProblemDetails?> ValidateRequestsAsync(List<CreateKeepableRequest> requests)
    {
        var modelState = new ModelStateDictionary();

        for (var i = 0; i < requests.Count; i++)
        {
            var query = DbContext.SelfUnverifyKeepables.Where(o => o.Group == requests[i].Group && o.Name == requests[i].Name);
            if (!await ContextHelper.IsAnyAsync(query, CancellationToken))
                continue;

            modelState.AddModelError($"[{i}].Name", $"Keepable with name '{requests[i].Name}' in group '{requests[i].Group}' already exists.");
        }

        return modelState.IsValid ? null : new ValidationProblemDetails(modelState);
    }
}
