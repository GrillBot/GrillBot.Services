using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RemindService.Core.Entity;
using RemindService.Models.Request;
using RemindService.Models.Response;
using RemindService.Options;

namespace RemindService.Actions;

public class CopyReminderAction(
    ICounterManager counterManager,
    RemindServiceContext dbContext,
    CreateReminderAction _createReminderAction
) : ApiAction<RemindServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<CopyReminderRequest>(0);

        var message = await ContextHelper.ReadFirstOrDefaultEntityAsync(DbContext.RemindMessages.Where(o => o.Id == request.RemindId));
        var modelState = await CheckOriginalReminderAsync(message, request);

        if (!modelState.IsValid)
            return ApiResult.BadRequest(new ValidationProblemDetails(modelState));

        var result = await CreateCopyAsync(message!, request);
        return ApiResult.Ok(result);
    }

    private async Task<ModelStateDictionary> CheckOriginalReminderAsync(RemindMessage? message, CopyReminderRequest request)
    {
        var modelState = new ModelStateDictionary();

        if (message is null)
        {
            modelState.AddModelError(nameof(request.RemindId), "RemindModule/Copy/RemindNotFound");
            return modelState;
        }

        if (message.FromUserId == request.ToUserId || message.ToUserId == request.ToUserId)
            modelState.AddModelError("Remind", "RemindModule/Copy/SelfCopy");

        if (!string.IsNullOrEmpty(message.NotificationMessageId))
        {
            if (message.NotificationMessageId == AppOptions.FinishedUnsentMessageId)
                modelState.AddModelError("Remind", "RemindModule/Copy/WasCancelled");
            else
                modelState.AddModelError("Remind", "RemindModule/Copy/WasSent");
        }

        if (message.ToUserId != request.ToUserId && await ExistsCopyAsync(message.CommandMessageId, request.ToUserId))
            modelState.AddModelError("Remind", "RemindModule/Copy/CopyExists");

        return modelState;
    }

    private async Task<bool> ExistsCopyAsync(string commandMessageId, string toUserId)
    {
        var query = DbContext.RemindMessages.Where(o => o.CommandMessageId == commandMessageId && o.ToUserId == toUserId);
        return await ContextHelper.IsAnyAsync(query);
    }

    private async Task<CreateReminderResult> CreateCopyAsync(RemindMessage message, CopyReminderRequest request)
    {
        var createRequest = new CreateReminderRequest
        {
            CommandMessageId = message.CommandMessageId,
            FromUserId = message.FromUserId,
            Language = request.Language,
            Message = message.Message,
            NotifyAtUtc = message.NotifyAtUtc,
            ToUserId = request.ToUserId
        };

        _createReminderAction.Init(HttpContext, [createRequest], CurrentUser);
        var result = await _createReminderAction.ProcessAsync();

        return (CreateReminderResult)result.Data!;
    }
}
