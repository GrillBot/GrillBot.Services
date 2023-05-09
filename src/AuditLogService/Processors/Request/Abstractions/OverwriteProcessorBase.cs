using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using Discord;
using GrillBot.Core.Extensions;

namespace AuditLogService.Processors.Request.Abstractions;

public abstract class OverwriteProcessorBase : BatchRequestProcessorBase
{
    protected OverwriteProcessorBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override bool IsValidItem(IAuditLogEntry entry, LogRequest request)
        => request.DiscordId.ToUlong() == entry.Id;

    protected static OverwriteInfo CreateOverwriteInfo(Overwrite overwrite)
    {
        return new OverwriteInfo
        {
            Id = Guid.NewGuid(),
            Target = overwrite.TargetType,
            AllowValue = overwrite.Permissions.AllowValue.ToString(),
            TargetId = overwrite.TargetId.ToString(),
            DenyValue = overwrite.Permissions.DenyValue.ToString()
        };
    }
}
