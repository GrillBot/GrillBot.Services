using GrillBot.Core.Validation;
using Microsoft.Extensions.Options;
using RemindService.Models.Request;
using RemindService.Options;
using System.ComponentModel.DataAnnotations;

namespace RemindService.Validators;

public class CreateReminderRequestValidator : ModelValidator<CreateReminderRequest>
{
    protected override IEnumerable<Func<CreateReminderRequest, ValidationContext, IEnumerable<ValidationResult>>> GetValidations()
    {
        yield return ValidateTime;
    }

    private IEnumerable<ValidationResult> ValidateTime(CreateReminderRequest request, ValidationContext context)
    {
        var utcCheck = CheckUtcDateTime(request.NotifyAtUtc, nameof(request.NotifyAtUtc));
        if (utcCheck is not null) return [utcCheck];

        var now = DateTime.UtcNow;
        if (request.NotifyAtUtc < now)
            return [new ValidationResult("RemindModule/Create/Validation/MustInFuture", [nameof(request.NotifyAtUtc)])];

        var options = context.GetRequiredService<IOptions<AppOptions>>();
        if ((request.NotifyAtUtc - now) <= options.Value.MinimalTime)
            return [new ValidationResult("RemindModule/Create/Validation/MinimalTime", [nameof(request.NotifyAtUtc)])];

        return [];
    }
}
