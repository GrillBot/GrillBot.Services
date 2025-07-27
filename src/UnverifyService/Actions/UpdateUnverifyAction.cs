using GrillBot.Core.Extensions;
using GrillBot.Core.Extensions.Discord;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.GrillBot.Models;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Core.Services.UserMeasures.Models.Events;
using GrillBot.Services.Common.Discord;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;
using UnverifyService.Models.Events;
using UnverifyService.Models.Request;

namespace UnverifyService.Actions;

public class UpdateUnverifyAction(
    IServiceProvider serviceProvider,
    IRabbitPublisher _rabbitPublisher,
    DiscordManager _discordManager
) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        if (!CurrentUser.IsLogged)
            return new ApiResult(StatusCodes.Status403Forbidden, new { Message = "User is not logged. Missing Authorization token" });

        var request = GetParameter<UpdateUnverifyRequest>(0);
        var unverify = await FindActiveUnverifyAsync(request);

        if (unverify is null)
            return ApiResult.NotFound(new LocalizedMessageContent("Unverify/Update/UnverifyNotFound", []));

        if ((unverify.EndAtUtc - DateTime.UtcNow).TotalSeconds <= 30.0)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(nameof(UpdateUnverifyRequest.UserId), "Unverify/Update/NotEnoughTime");

            return ApiResult.BadRequest(modelState);
        }

        var logItem = await WriteToLogAsync(unverify, request);

        unverify.StartAtUtc = logItem.UpdateOperation!.NewStartAtUtc;
        unverify.EndAtUtc = logItem.UpdateOperation.NewEndAtUtc;

        await ContextHelper.SaveChangesAsync(CancellationToken);
        await NotifyUserMeasuresAsync(unverify);
        await SendUserNotificationAsync(unverify, logItem);
        await RecalculateMetricsAsync();
        return await CreateResultAsync(unverify, logItem);
    }

    private async Task<ActiveUnverify?> FindActiveUnverifyAsync(UpdateUnverifyRequest request)
    {
        var guildId = request.GuildId.ToUlong();
        var userId = request.UserId.ToUlong();

        var query = DbContext.ActiveUnverifies
            .Include(o => o.LogItem)
            .Where(o => o.LogItem.ToUserId == userId && o.LogItem.GuildId == guildId);

        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query, CancellationToken);
    }

    private async Task<UnverifyLogItem> WriteToLogAsync(ActiveUnverify unverify, UpdateUnverifyRequest request)
    {
        var logItem = new UnverifyLogItem
        {
            CreatedAt = DateTime.UtcNow,
            FromUserId = CurrentUser.Id.ToUlong(),
            GuildId = unverify.LogItem.GuildId,
            Id = Guid.NewGuid(),
            OperationType = UnverifyOperationType.Update,
            ParentLogItem = unverify.LogItem,
            ParentLogItemId = unverify.LogItem.Id,
            ToUserId = unverify.LogItem.ToUserId,
            UpdateOperation = new UnverifyLogUpdateOperation
            {
                NewEndAtUtc = request.NewEndAtUtc,
                NewStartAtUtc = DateTime.UtcNow,
                Reason = request.Reason
            }
        };

        await ContextHelper.DbContext.AddAsync(logItem, CancellationToken);
        return logItem;
    }

    private Task NotifyUserMeasuresAsync(ActiveUnverify unverify)
    {
        var payload = new UnverifyModifyPayload(unverify.LogItem.LogNumber, unverify.EndAtUtc);
        return _rabbitPublisher.PublishAsync(payload, cancellationToken: CancellationToken);
    }

    private async Task SendUserNotificationAsync(ActiveUnverify unverify, UnverifyLogItem updateLogItem)
    {
        var guild = await _discordManager.GetGuildAsync(unverify.LogItem.GuildId, cancellationToken: CancellationToken);
        var userQuery = ContextHelper.DbContext.Users.Where(o => o.Id == unverify.LogItem.ToUserId).Select(o => o.Language ?? "cs");
        var userLocale = (await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, CancellationToken)) ?? "cs";

        var localizationArgs = new List<string>
        {
            guild?.Name ?? "Unknonwn guild",
            $"DateTime:{unverify.EndAtUtc:o}"
        };

        var localizationKey = "Unverify/Message/PrivateUpdate";

        if (!string.IsNullOrEmpty(updateLogItem.UpdateOperation!.Reason))
        {
            localizationKey = "Unverify/Message/PrivateUpdateWithReason";
            localizationArgs.Add(updateLogItem.UpdateOperation.Reason);
        }

        var payload = new DiscordSendMessagePayload(
            guildId: null,
            channelId: unverify.LogItem.ToUserId,
            content: new LocalizedMessageContent(localizationKey, [.. localizationArgs]),
            attachments: [],
            "Unverify"
        );

        payload.WithLocalization(locale: userLocale);
        await _rabbitPublisher.PublishAsync(payload, cancellationToken: CancellationToken);
    }

    private async Task<ApiResult> CreateResultAsync(ActiveUnverify unverify, UnverifyLogItem updateLogItem)
    {
        var targetUser = await _discordManager.GetGuildUserAsync(unverify.LogItem.GuildId, unverify.LogItem.ToUserId);

        var localizationKey = "Unverify/Message/UpdateToChannel";
        var localizationArgs = new List<string>
        {
            targetUser!.GetDisplayName(),
            $"DateTime:{unverify.EndAtUtc:o}"
        };

        if (!string.IsNullOrEmpty(updateLogItem.UpdateOperation!.Reason))
        {
            localizationKey = "Unverify/Message/UpdateToChannelWithReason";
            localizationArgs.Add(updateLogItem.UpdateOperation!.Reason);
        }

        return ApiResult.Ok(new LocalizedMessageContent(localizationKey, [.. localizationArgs]));
    }

    private Task RecalculateMetricsAsync()
        => _rabbitPublisher.PublishAsync(new RecalculateMetricsMessage(), cancellationToken: CancellationToken);
}
