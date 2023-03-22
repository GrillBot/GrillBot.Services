namespace PointsService.Validation;

public static class ValidatorExtensions
{
    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        return services
            .AddScoped<TransactionRequestValidator>();
    }
}
