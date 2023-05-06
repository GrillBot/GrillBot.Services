using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using MemberInfo = AuditLogService.Core.Entity.MemberInfo;

namespace AuditLogService.Processors.Request;

public class MemberUpdatedProcessor : RequestProcessorBase
{
    public MemberUpdatedProcessor(DiscordManager discordManager) : base(discordManager)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type != LogType.MemberUpdated)
            return;

        if (request.MemberUpdated!.IsApiUpdate())
        {
            var before = CreateMemberInfo(request.MemberUpdated.UserId, request.MemberUpdated.SelfUnverifyMinimalTime!.Before, request.MemberUpdated.Flags!.Before);
            var after = CreateMemberInfo(request.MemberUpdated.UserId, request.MemberUpdated.SelfUnverifyMinimalTime!.After, request.MemberUpdated.Flags!.After);

            entity.MemberUpdated = new MemberUpdated
            {
                Before = before,
                BeforeId = before.Id,
                AfterId = after.Id,
                After = after
            };
        }
        else
        {
            var auditLogs = await DiscordManager.GetAuditLogsAsync(entity.GuildId!.ToUlong(), actionType: ActionType.MemberUpdated);
            var logItem = auditLogs.FirstOrDefault(o => ((MemberUpdateAuditLogData)o.Data).Target.Id == request.MemberUpdated.UserId.ToUlong());
            if (logItem is null)
            {
                entity.CanCreate = false;
                return;
            }

            var logData = (MemberUpdateAuditLogData)logItem.Data;
            var before = CreateMemberInfo(logData.Target.Id, logData.Before);
            var after = CreateMemberInfo(logData.Target.Id, logData.After);

            entity.DiscordId = logItem.Id.ToString();
            entity.UserId = logItem.User.Id.ToString();
            entity.MemberUpdated = new MemberUpdated
            {
                After = after,
                Before = before,
                BeforeId = before.Id,
                AfterId = after.Id
            };
        }
    }

    private static MemberInfo CreateMemberInfo(ulong userId, Discord.Rest.MemberInfo info)
    {
        return new MemberInfo
        {
            UserId = userId.ToString(),
            Id = Guid.NewGuid(),
            Nickname = info.Nickname,
            IsDeaf = info.Deaf,
            IsMuted = info.Mute
        };
    }

    private static MemberInfo CreateMemberInfo(string userId, string? selfUnverifyMinimalTime, int? flags)
    {
        return new MemberInfo
        {
            Flags = flags,
            Id = Guid.NewGuid(),
            UserId = userId,
            SelfUnverifyMinimalTime = selfUnverifyMinimalTime
        };
    }
}
