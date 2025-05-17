using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using System.Net;

namespace AuditLogService.Processors.Request;

public class ApiRequestProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
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
            ActionName = apiRequest.ActionName.Replace("Async", ""),
            ControllerName = apiRequest.ControllerName.Replace("Controller", ""),
            EndAt = apiRequest.EndAt,
            StartAt = apiRequest.StartAt,
            TemplatePath = apiRequest.TemplatePath,
            ApiGroupName = apiRequest.ApiGroupName,
            Result = apiRequest.Result,
            IsSuccess = successStatusCodes.Contains(apiRequest.Result),
            RequestDate = DateOnly.FromDateTime(apiRequest.EndAt),
            Role = apiRequest.Role,
            Duration = (long)Math.Round((apiRequest.EndAt - apiRequest.StartAt).TotalMilliseconds)
        };

        if (apiRequest.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            entity.ApiRequest.ForwardedIp = forwardedFor;

        return Task.CompletedTask;
    }
}
