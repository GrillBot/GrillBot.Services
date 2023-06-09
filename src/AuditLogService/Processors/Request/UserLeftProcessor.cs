using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;

namespace AuditLogService.Processors.Request;

public class UserLeftProcessor : RequestProcessorBase
{
    public UserLeftProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var ban = await DiscordManager.GetBanAsync(request.GuildId.ToUlong(), request.UserLeft!.UserId.ToUlong());
        var logActionType = ban is not null ? ActionType.Ban : ActionType.Kick;
        var logItem = await FindAuditLogAsync(request, logActionType);

        if (logItem is not null)
        {
            entity.UserId = logItem.User.Id.ToString();
            entity.DiscordId = logItem.Id.ToString();
            entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
        }

        entity.UserLeft = new UserLeft
        {
            UserId = request.UserLeft.UserId,
            BanReason = ban?.Reason,
            IsBan = ban is not null,
            MemberCount = request.UserLeft.MemberCount
        };
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
    {
        var timeLimit = DateTime.UtcNow.AddMinutes(-1);
        var targetId = entry.Action is ActionType.Ban ? ((BanAuditLogData)entry.Data).Target.Id : ((KickAuditLogData)entry.Data).Target.Id;

        return entry.CreatedAt.UtcDateTime >= timeLimit && targetId == request.UserLeft!.UserId.ToUlong();
    }
}
