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

        if (!string.IsNullOrEmpty(apiRequest.Role))
        {
            entity.ApiRequest.Role = apiRequest.Role;
        }
        else if (apiRequest.ApiGroupName == "V1")
        {
            if (apiRequest.Identification.StartsWith("ApiV1(Private/"))
                entity.ApiRequest.Role = "Admin";
            else if (apiRequest.Identification.StartsWith("ApiV1(Public/"))
                entity.ApiRequest.Role = "User";
        }

        return Task.CompletedTask;
    }
}
