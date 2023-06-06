using System.ComponentModel.DataAnnotations;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;

namespace AuditLogService.Validators;

public class LogRequestValidator : ModelValidator<LogRequest>
{
    private static readonly HashSet<LogType> TypesWithRequiredGuildId = new()
    {
        LogType.ThreadUpdated, LogType.GuildUpdated, LogType.EmoteDeleted, LogType.ChannelCreated, LogType.ChannelDeleted, LogType.ThreadDeleted, LogType.MemberUpdated,
        LogType.ChannelUpdated, LogType.UserLeft, LogType.MessageDeleted, LogType.OverwriteUpdated, LogType.MemberRoleUpdated, LogType.OverwriteCreated, LogType.OverwriteDeleted,
        LogType.InteractionCommand
    };

    private static readonly HashSet<LogType> TypesWithRequiredUserId = new()
    {
        LogType.InteractionCommand
    };
    
    private static readonly HashSet<LogType> TypesWithRequriedChannelId = new()
    {
        LogType.ChannelCreated, LogType.ChannelDeleted, LogType.ChannelUpdated, LogType.InteractionCommand
    };
    
    private static readonly HashSet<LogType> TypesWithRequriedDiscordId = new()
    {
        LogType.MemberRoleUpdated
    };

    protected override IEnumerable<Func<LogRequest, ValidationContext, IEnumerable<ValidationResult>>> GetValidations()
    {
        yield return ValidateDateTime;
        yield return ValidateDataBindings;
        yield return ValidateIdBindings;
    }

    private static IEnumerable<ValidationResult> ValidateDateTime(LogRequest request, ValidationContext _)
    {
        if (request.CreatedAt.HasValue && request.CreatedAt.Value.Kind != DateTimeKind.Utc)
            yield return new ValidationResult("Only UTC value is allowed.", new[] { nameof(request.CreatedAt) });
    }

    private static IEnumerable<ValidationResult> ValidateDataBindings(LogRequest request, ValidationContext _)
    {
        // <Type, Object>
        var requiredBindings = new Dictionary<LogType, object?>()
        {
            { LogType.Info, request.LogMessage },
            { LogType.Warning, request.LogMessage },
            { LogType.Error, request.LogMessage },
            { LogType.ChannelCreated, request.ChannelInfo },
            { LogType.ChannelDeleted, request.ChannelInfo },
            { LogType.ChannelUpdated, request.ChannelUpdated },
            { LogType.EmoteDeleted, request.DeletedEmote },
            // OverwriteCreated, OverwriteDeleted, OverwriteUpdated not have any concrete model.
            { LogType.Unban, request.Unban },
            { LogType.MemberUpdated, request.MemberUpdated },
            // MemberRoleUpdated not have any concrete model.
            { LogType.GuildUpdated, request.GuildUpdated },
            { LogType.UserLeft, request.UserLeft },
            { LogType.UserJoined, request.UserJoined },
            { LogType.MessageEdited, request.MessageEdited },
            { LogType.MessageDeleted, request.MessageDeleted },
            { LogType.InteractionCommand, request.InteractionCommand },
            { LogType.ThreadDeleted, request.ThreadInfo },
            { LogType.JobCompleted, request.JobExecution },
            { LogType.Api, request.ApiRequest },
            { LogType.ThreadUpdated, request.ThreadUpdated },
        };

        if (requiredBindings.TryGetValue(request.Type, out var requestData) && requestData is null)
            yield return new ValidationResult($"Missing data for type {request.Type}");

        if (requiredBindings.Values.Count(o => o is not null) > 1)
            yield return new ValidationResult("Only one property with data can be setted.");
    }

    private static IEnumerable<ValidationResult> ValidateIdBindings(LogRequest request, ValidationContext _)
    {
        if (string.IsNullOrEmpty(request.GuildId) && TypesWithRequiredGuildId.Contains(request.Type))
            yield return new ValidationResult($"Missing required property GuildId for type {request.Type}.", new[] { nameof(request.GuildId) });

        if (string.IsNullOrEmpty(request.UserId) && TypesWithRequiredUserId.Contains(request.Type))
            yield return new ValidationResult($"Missing required property UserId for type {request.Type}.", new[] { nameof(request.UserId) });

        if (string.IsNullOrEmpty(request.ChannelId) && TypesWithRequriedChannelId.Contains(request.Type))
            yield return new ValidationResult($"Missing required property ChannelId for type {request.Type}.", new[] { nameof(request.ChannelId) });

        if (string.IsNullOrEmpty(request.DiscordId) && TypesWithRequriedDiscordId.Contains(request.Type))
            yield return new ValidationResult($"Missing required property DiscordId for type {request.Type}.", new[] { nameof(request.DiscordId) });
    }
}
