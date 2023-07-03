namespace PointsService.Validation;

public static class ValidatorExtensions
{
    public static void AddValidations(this IServiceCollection services)
    {
        services
            .AddScoped<TransactionRequestValidator>();
    }
}
