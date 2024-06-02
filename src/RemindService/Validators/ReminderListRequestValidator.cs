using GrillBot.Core.Validation;
using RemindService.Models.Request;
using System.ComponentModel.DataAnnotations;

namespace RemindService.Validators;

public class ReminderListRequestValidator : ModelValidator<ReminderListRequest>
{
    protected override IEnumerable<Func<ReminderListRequest, ValidationContext, IEnumerable<ValidationResult>>> GetValidations()
    {
        yield return ValidateDateTimeRanges;
    }

    private static IEnumerable<ValidationResult> ValidateDateTimeRanges(ReminderListRequest request, ValidationContext _)
    {
        var notifyFromUtcCheck = CheckUtcDateTime(request.NotifyAtFromUtc, nameof(request.NotifyAtFromUtc));
        if (notifyFromUtcCheck is not null) yield return notifyFromUtcCheck;

        var notifyToUtcCheck = CheckUtcDateTime(request.NotifyAtToUtc, nameof(request.NotifyAtToUtc));
        if (notifyToUtcCheck is not null) yield return notifyToUtcCheck;

        if (request.NotifyAtFromUtc.HasValue && request.NotifyAtToUtc.HasValue)
        {
            var rangeCheck = CheckDateTimeRange(request.NotifyAtFromUtc, request.NotifyAtToUtc, nameof(request.NotifyAtFromUtc), nameof(request.NotifyAtToUtc));
            if (rangeCheck is not null) yield return rangeCheck;
        }
    }
}
