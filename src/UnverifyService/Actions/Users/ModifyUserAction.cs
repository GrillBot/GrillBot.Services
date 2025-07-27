using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Infrastructure.Api;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Events;
using UnverifyService.Models.Request.Users;

namespace UnverifyService.Actions.Users;

public class ModifyUserAction(
    IServiceProvider serviceProvider,
    IRabbitPublisher _rabbitPublisher
) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        if (!CurrentUser.IsLogged)
            return new ApiResult(StatusCodes.Status403Forbidden, new { Message = "User is not logged. Missing Authorization token" });

        var userId = GetParameter<ulong>(0);
        var request = GetParameter<ModifyUserRequest>(1);

        var userQuery = ContextHelper.DbContext.Users.Where(o => o.Id == userId);
        var userEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, CancellationToken);

        if (userEntity is null)
        {
            userEntity = new User
            {
                Id = userId
            };

            await ContextHelper.DbContext.AddAsync(userEntity);
        }

        var oldSelfUnverifyTime = userEntity.SelfUnverifyMinimalTime;
        userEntity.SelfUnverifyMinimalTime = request.SelfUnverifyMinimalTime;
        userEntity.IsBotAdmin = request.IsBotAdmin;

        var changedRows = await ContextHelper.SaveChangesAsync(CancellationToken);

        if (changedRows > 0)
        {
            await NotifyAuditLogAsync(userEntity, oldSelfUnverifyTime);
            await RecalculateMetricsAsync();
        }

        return ApiResult.Ok();
    }

    private Task NotifyAuditLogAsync(User userEntity, TimeSpan? oldSelfUnverifyTime)
    {
        var logRequest = new LogRequest(LogType.MemberUpdated, DateTime.UtcNow, null, CurrentUser.Id)
        {
            MemberUpdated = new MemberUpdatedRequest
            {
                SelfUnverifyMinimalTime = new DiffRequest<string?>
                {
                    After = userEntity.SelfUnverifyMinimalTime?.ToString("c"),
                    Before = oldSelfUnverifyTime?.ToString("c")
                },
                UserId = userEntity.Id.ToString()
            }
        };

        return _rabbitPublisher.PublishAsync(new CreateItemsMessage(logRequest), cancellationToken: CancellationToken);
    }

    private Task RecalculateMetricsAsync()
        => _rabbitPublisher.PublishAsync(new RecalculateMetricsMessage(), cancellationToken: CancellationToken);
}
