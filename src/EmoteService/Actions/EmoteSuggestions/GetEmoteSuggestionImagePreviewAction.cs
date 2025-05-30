using EmoteService.Core.Entity;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.EmoteSuggestions;

public class GetEmoteSuggestionImagePreviewAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var suggestionId = GetParameter<Guid>(0);

        var query = DbContext.EmoteSuggestions.AsNoTracking()
            .Where(o => o.Id == suggestionId)
            .Select(o => new { o.Image, o.IsAnimated });

        var image = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        if (image is null)
            return ApiResult.NotFound();

        var contentType = image.IsAnimated ? "image/gif" : "image/png";
        return ApiResult.Ok(new FileContentResult(image.Image, contentType));
    }
}
