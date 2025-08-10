using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Services.GrillBot.Models;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;

namespace UnverifyService.Actions;

public class CheckRecoveryRequirementsAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var logId = GetOptionalParameter<Guid>(0);
        var logNumber = GetOptionalParameter<long>(1);

        var errorMessage = ValidateInput(logId, logNumber);
        if (errorMessage is not null)
            return CreateResult(errorMessage);

        var logItem = await FindLogItemAsync(logId, logNumber);
        if (logItem is null)
            return CreateResult(new LocalizedMessageContent("Unverify/Validation/Recovery/LogNotFound", []));

        errorMessage = await ValidateActiveUnverifyAsync(logItem);
        return CreateResult(errorMessage);
    }

    private static ApiResult CreateResult(LocalizedMessageContent? errorMessage)
    {
        return errorMessage is null ?
            ApiResult.Ok() :
            new ApiResult(StatusCodes.Status400BadRequest, errorMessage);
    }

    private static LocalizedMessageContent? ValidateInput(Guid? id, long? number)
    {
        return id is null && number is null ?
            new LocalizedMessageContent("Unverify/Validation/Recovery/LogIdOrNumberRequired", []) :
            null;
    }

    private Task<UnverifyLogItem?> FindLogItemAsync(Guid? logId, long? logNumber)
    {
        var query = DbContext.LogItems.AsNoTracking()
            .Where(o => (o.OperationType == UnverifyOperationType.Unverify || o.OperationType == UnverifyOperationType.SelfUnverify) && o.SetOperation != null);

        if (logNumber is not null)
            query = query.Where(o => o.LogNumber == logNumber.Value);
        else if (logId is not null)
            query = query.Where(o => o.Id == logId.Value);
        else
            return Task.FromResult<UnverifyLogItem?>(null);

        return ContextHelper.ReadFirstOrDefaultEntityAsync(query, CancellationToken);
    }

    private async Task<LocalizedMessageContent?> ValidateActiveUnverifyAsync(UnverifyLogItem item)
    {
        var query = DbContext.ActiveUnverifies.AsNoTracking()
            .Where(o => o.LogItem.GuildId == item.GuildId && o.LogItem.ToUserId == item.ToUserId);

        return (await ContextHelper.IsAnyAsync(query, CancellationToken)) ?
            new LocalizedMessageContent("Unverify/Validation/Recovery/ActiveUnverifyExists", []) :
            null;
    }
}
