namespace GrillBot.Services.Common.Infrastructure.Api.OpenApi.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerRequireAuthorizationAttribute : Attribute
{
}
