using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using MemberInfo = AuditLogService.Core.Entity.MemberInfo;

namespace AuditLogService.Processors.Request;

public class MemberUpdatedProcessor : RequestProcessorBase
{
    public MemberUpdatedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
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
            var logItem = await FindAuditLogAsync(request);
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
            entity.CreatedAt = logItem.CreatedAt.UtcDateTime;
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

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
        => ((MemberUpdateAuditLogData)entry.Data).Target.Id == request.MemberUpdated!.UserId.ToUlong();
}
