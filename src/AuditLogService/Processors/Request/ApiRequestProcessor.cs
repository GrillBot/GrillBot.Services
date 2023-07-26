using AuditLogService.Core.Entity;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors.Request.Abstractions;
using System.Net;

namespace AuditLogService.Processors.Request;

public class ApiRequestProcessor : RequestProcessorBase
{
    public ApiRequestProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var successStatusCodes = Enum.GetValues<HttpStatusCode>()
            .Where(o => o < HttpStatusCode.BadRequest)
            .Select(o => $"{(int)o} ({o})")
            .ToHashSet();

        var apiRequest = request.ApiRequest!;
        entity.ApiRequest = new ApiRequest
        {
            Headers = apiRequest.Headers,
            Identification = apiRequest.Identification,
            Ip = apiRequest.Ip,
            Language = apiRequest.Language,
            Method = apiRequest.Method,
            Parameters = apiRequest.Parameters,
            Path = apiRequest.Path,
            ActionName = apiRequest.ActionName,
            ControllerName = apiRequest.ControllerName,
            EndAt = apiRequest.EndAt,
            StartAt = apiRequest.StartAt,
            TemplatePath = apiRequest.TemplatePath,
            ApiGroupName = apiRequest.ApiGroupName,
            Result = apiRequest.Result,
            IsSuccess = successStatusCodes.Contains(apiRequest.Result),
            RequestDate = DateOnly.FromDateTime(apiRequest.EndAt),
        };

        SetUserRole(apiRequest, entity.ApiRequest);
        SetForwardedIp(apiRequest, entity.ApiRequest);

        return Task.CompletedTask;
    }

    private static void SetUserRole(ApiRequestRequest request, ApiRequest entity)
    {
        if (!string.IsNullOrEmpty(request.Role))
        {
            entity.Role = request.Role;
        }
        else if (request.ApiGroupName == "V1")
        {
            if (request.Identification.StartsWith("ApiV1(Private/"))
                entity.Role = "Admin";
            else if (request.Identification.StartsWith("ApiV1(Public/"))
                entity.Role = "User";
        }
    }

    private static void SetForwardedIp(ApiRequestRequest request, ApiRequest entity)
    {
        if (request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            entity.ForwardedIp = forwardedFor;
    }
}
