using GrillBot.Services.Common.Infrastructure.Api.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GrillBot.Services.Common.Infrastructure.Api.OpenApi.Filters;

internal class RequireAuthorizationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.ApiDescription.CustomAttributes()
            .OfType<SwaggerRequireAuthorizationAttribute>()
            .FirstOrDefault();

        if (attributes is null)
            return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Description = "Authorization header",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Pattern = @"^Bearer\s([A-Za-z0-9_-]+)\.([A-Za-z0-9_-]+)\.([A-Za-z0-9_-]+)$"
            }
        });
    }
}
