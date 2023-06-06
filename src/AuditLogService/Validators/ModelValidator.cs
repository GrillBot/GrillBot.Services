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
}
