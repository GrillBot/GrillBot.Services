using System.ComponentModel.DataAnnotations;
using AuditLogService.Models.Request.Search;

namespace AuditLogService.Validators;

public class SearchRequestValidator : ModelValidator<SearchRequest>
{
    protected override IEnumerable<Func<SearchRequest, ValidationContext, IEnumerable<ValidationResult>>> GetValidations()
    {
        yield return ValidateRanges;
        yield return ValidateTypes;
    }

    private static IEnumerable<ValidationResult> ValidateTypes(SearchRequest request, ValidationContext _)
    {
        if (request.IgnoreTypes.Intersect(request.ShowTypes).Any())
            yield return new ValidationResult("You cannot filter and exclude the same types at the same time.", new[] { nameof(request.ShowTypes), nameof(request.IgnoreTypes) });
    }

    private static IEnumerable<ValidationResult> ValidateRanges(SearchRequest request, ValidationContext _)
    {
        if (request.AdvancedSearch is not null)
        {
            var rangeCheck = CheckDurationRange(request.AdvancedSearch.Api?.DurationFrom, request.AdvancedSearch.Api?.DurationTo, nameof(request.AdvancedSearch.Api.DurationFrom),
                nameof(request.AdvancedSearch.Api.DurationTo));
            if (rangeCheck is not null) yield return rangeCheck;

            rangeCheck = CheckDurationRange(request.AdvancedSearch.Job?.DurationFrom, request.AdvancedSearch.Job?.DurationTo, nameof(request.AdvancedSearch.Job.DurationFrom),
                nameof(request.AdvancedSearch.Job.DurationTo));
            if (rangeCheck is not null) yield return rangeCheck;

            rangeCheck = CheckDurationRange(request.AdvancedSearch.Interaction?.DurationFrom, request.AdvancedSearch.Interaction?.DurationTo, nameof(request.AdvancedSearch.Interaction.DurationFrom),
                nameof(request.AdvancedSearch.Interaction.DurationTo));
            if (rangeCheck is not null) yield return rangeCheck;
        }

        var utcCheck = CheckUtcDateTime(request.CreatedFrom, nameof(request.CreatedFrom));
        if (utcCheck is not null) yield return utcCheck;

        utcCheck = CheckUtcDateTime(request.CreatedTo, nameof(request.CreatedTo));
        if (utcCheck is not null) yield return utcCheck;

        if (request.CreatedFrom > request.CreatedTo)
            yield return new ValidationResult("Unallowed interval of created from date and created to date.", new[] { nameof(request.CreatedFrom), nameof(request.CreatedTo) });
    }
}
