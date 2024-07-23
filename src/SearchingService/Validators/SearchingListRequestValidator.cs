using GrillBot.Core.Validation;
using SearchingService.Models.Request;
using System.ComponentModel.DataAnnotations;

namespace SearchingService.Validators;

public class SearchingListRequestValidator : ModelValidator<SearchingListRequest>
{
    protected override IEnumerable<Func<SearchingListRequest, ValidationContext, IEnumerable<ValidationResult>>> GetValidations()
    {
        yield return ValidateDateTimeRanges;
    }

    private static IEnumerable<ValidationResult> ValidateDateTimeRanges(SearchingListRequest request, ValidationContext _)
    {
        var created = CheckDateTimeRangePair(request.CreatedFrom, nameof(request.CreatedFrom), request.CreatedTo, nameof(request.CreatedTo));
        var validity = CheckDateTimeRangePair(request.ValidFrom, nameof(request.ValidFrom), request.ValidTo, nameof(request.ValidTo));

        return created.Concat(validity);
    }

    private static IEnumerable<ValidationResult> CheckDateTimeRangePair(DateTime? from, string fromProperty, DateTime? to, string toProperty)
    {
        var fromUtcCheck = CheckUtcDateTime(from, fromProperty);
        if (fromUtcCheck is not null) yield return fromUtcCheck;

        var toUtcCheck = CheckUtcDateTime(to, toProperty);
        if (toUtcCheck is not null) yield return toUtcCheck;

        var rangeCheck = CheckDateTimeRange(from, to, fromProperty, toProperty);
        if (rangeCheck is not null) yield return rangeCheck;
    }
}
