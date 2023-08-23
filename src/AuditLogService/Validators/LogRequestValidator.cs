using System.ComponentModel.DataAnnotations;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request.CreateItems;
using GrillBot.Core.Validation;

namespace AuditLogService.Validators;

public class LogRequestValidator : ModelValidator<LogRequest>
{
    private static readonly HashSet<LogType> TypesWithRequiredGuildId = new()
    {
        LogType.ThreadUpdated, LogType.GuildUpdated, LogType.EmoteDeleted, LogType.ChannelCreated, LogType.ChannelDeleted, LogType.ThreadDeleted, LogType.ChannelUpdated, LogType.UserLeft,
        LogType.MessageDeleted, LogType.OverwriteUpdated, LogType.MemberRoleUpdated, LogType.OverwriteCreated, LogType.OverwriteDeleted
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
        yield return ValidateThreadUpdatedProperties;
        yield return ValidateInteractionCommandProperties;
    }

    private static IEnumerable<ValidationResult> ValidateDateTime(LogRequest request, ValidationContext _)
    {
        var dateTimeValidations = new List<ValidationResult?>
        {
            CheckUtcDateTime(request.CreatedAt, nameof(request.CreatedAt))
        };

        if (request.ApiRequest is not null)
        {
            dateTimeValidations.Add(CheckUtcDateTime(request.ApiRequest.StartAt, nameof(request.ApiRequest.StartAt)));
            dateTimeValidations.Add(CheckUtcDateTime(request.ApiRequest.EndAt, nameof(request.ApiRequest.EndAt)));
            dateTimeValidations.Add(CheckDateTimeRange(request.ApiRequest.StartAt, request.ApiRequest.EndAt, nameof(request.ApiRequest.StartAt), nameof(request.ApiRequest.EndAt)));
        }

        if (request.JobExecution is not null)
        {
            dateTimeValidations.Add(CheckUtcDateTime(request.JobExecution.StartAt, nameof(request.JobExecution.StartAt)));
            dateTimeValidations.Add(CheckUtcDateTime(request.JobExecution.EndAt, nameof(request.JobExecution.EndAt)));
            dateTimeValidations.Add(CheckDateTimeRange(request.JobExecution.StartAt, request.JobExecution.EndAt, nameof(request.JobExecution.StartAt), nameof(request.JobExecution.EndAt)));
        }

        if (request.MessageDeleted is not null)
            dateTimeValidations.Add(CheckUtcDateTime(request.MessageDeleted.MessageCreatedAt, nameof(request.MessageDeleted.MessageCreatedAt)));

        return dateTimeValidations.Where(o => o is not null)!;
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

        if (requiredBindings.Values.Where(o => o is not null).DistinctBy(o => o!.GetType().FullName).Count() > 1)
            yield return new ValidationResult("Only one property with data can be setted.");
    }

    private static IEnumerable<ValidationResult> ValidateIdBindings(LogRequest request, ValidationContext _)
    {
        if (string.IsNullOrEmpty(request.GuildId))
        {
            if (TypesWithRequiredGuildId.Contains(request.Type))
                yield return new ValidationResult($"Missing required property GuildId for type {request.Type}.", new[] { nameof(request.GuildId) });
            else if (request.MemberUpdated?.IsApiUpdate() == false)
                yield return new ValidationResult("Missing required property GuildId for type MemberUpdated.", new[] { nameof(request.GuildId) });
            else if (request.Type is LogType.InteractionCommand && !IsAllowedDmInteraction(request.InteractionCommand!))
                yield return new ValidationResult("Missing required property GuildId for type InteractionCommand.", new[] { nameof(request.GuildId) });
        }

        if (string.IsNullOrEmpty(request.UserId) && TypesWithRequiredUserId.Contains(request.Type))
            yield return new ValidationResult($"Missing required property UserId for type {request.Type}.", new[] { nameof(request.UserId) });

        if (string.IsNullOrEmpty(request.ChannelId) && TypesWithRequriedChannelId.Contains(request.Type))
            yield return new ValidationResult($"Missing required property ChannelId for type {request.Type}.", new[] { nameof(request.ChannelId) });

        if (string.IsNullOrEmpty(request.DiscordId) && TypesWithRequriedDiscordId.Contains(request.Type))
            yield return new ValidationResult($"Missing required property DiscordId for type {request.Type}.", new[] { nameof(request.DiscordId) });
    }

    private static IEnumerable<ValidationResult> ValidateThreadUpdatedProperties(LogRequest request, ValidationContext context)
    {
        if (request.Type is not LogType.ThreadUpdated)
            yield break;

        IEnumerable<ValidationResult> CheckThreadInfo(ThreadInfoRequest? threadInfoRequest)
        {
            var requiredAttribute = new RequiredAttribute();

            var validationResult = requiredAttribute.GetValidationResult(threadInfoRequest, context);
            if (validationResult is not null) yield return validationResult;

            validationResult = requiredAttribute.GetValidationResult(threadInfoRequest!.ThreadName, context);
            if (validationResult is not null) yield return validationResult;

            validationResult = requiredAttribute.GetValidationResult(threadInfoRequest.Type, context);
            if (validationResult is not null) yield return validationResult;
        }

        foreach (var validationError in CheckThreadInfo(request.ThreadUpdated?.Before))
            yield return validationError;

        foreach (var validationError in CheckThreadInfo(request.ThreadUpdated?.After))
            yield return validationError;
    }

    private static bool IsAllowedDmInteraction(InteractionCommandRequest interaction)
        => interaction is { ModuleName: "RemindModule" };

    private static IEnumerable<ValidationResult> ValidateInteractionCommandProperties(LogRequest request, ValidationContext context)
    {
        if (request.Type != LogType.InteractionCommand || request.InteractionCommand?.IsSuccess != false)
            yield break;

        if (request.InteractionCommand.CommandError is null)
            yield return new ValidationResult("CommandError property is mandatory if interaction failed.", new[] { nameof(request.InteractionCommand.CommandError) });

        if (string.IsNullOrEmpty(request.InteractionCommand.ErrorReason) && string.IsNullOrEmpty(request.InteractionCommand.Exception))
        {
            yield return new ValidationResult("Required ErrorReason or exception if interaction failed.",
                new[] { nameof(request.InteractionCommand.ErrorReason), nameof(request.InteractionCommand.Exception) });
        }
    }
}
