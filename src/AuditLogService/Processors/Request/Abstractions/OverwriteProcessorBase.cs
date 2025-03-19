﻿using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using Discord;
using GrillBot.Core.Extensions;

namespace AuditLogService.Processors.Request.Abstractions;

public abstract class OverwriteProcessorBase(IServiceProvider serviceProvider) : BatchRequestProcessorBase(serviceProvider)
{
    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
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
