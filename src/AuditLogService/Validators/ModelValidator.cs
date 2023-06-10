using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Validators;

public abstract class ModelValidator<TModel> where TModel : class
{
    public IEnumerable<ValidationResult> Validate(TModel model, ValidationContext context)
    {
        var validations = GetValidations();
        foreach (var validation in validations)
        {
            foreach (var result in validation(model, context))
                yield return result;
        }
    }

    protected abstract IEnumerable<Func<TModel, ValidationContext, IEnumerable<ValidationResult>>> GetValidations();

    protected static ValidationResult? CheckUtcDateTime(DateTime? dateTime, string propertyName)
    {
        if (dateTime.HasValue && dateTime.Value.Kind != DateTimeKind.Utc)
            return new ValidationResult("Only UTC value is allowed.", new[] { propertyName });
        return null;
    }

    protected static ValidationResult? CheckDurationRange(int? from, int? to, string fromPropertyName, string toPropertyName)
    {
        if (from is null || to is null)
            return null;

        return from > to ? new ValidationResult("Unallowed interval of duration range.", new[] { fromPropertyName, toPropertyName }) : null;
    }
}
