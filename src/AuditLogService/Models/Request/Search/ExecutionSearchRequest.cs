using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Models.Request.Search;

public class ExecutionSearchRequest : IAdvancedSearchRequest, IValidatableObject
{
    public string? ActionName { get; set; }
    public bool? Success { get; set; }

    public int? DurationFrom { get; set; }
    public int? DurationTo { get; set; }

    public bool IsSet()
        => !string.IsNullOrEmpty(ActionName) || Success is not null || DurationFrom is not null || DurationTo is not null;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DurationFrom is null || DurationTo is null)
            yield break;
        if (DurationFrom > DurationTo)
            yield return new ValidationResult("Unallowed interval of durations.", new[] { nameof(DurationFrom), nameof(DurationTo) });
    }
}
