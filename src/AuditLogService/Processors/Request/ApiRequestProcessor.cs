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
            Duration = (int)Math.Round((apiRequest.EndAt - apiRequest.StartAt).TotalMilliseconds)
        };

        ProcessHeaders(apiRequest, entity.ApiRequest);
        ProcessParameters(apiRequest, entity.ApiRequest);

        return Task.CompletedTask;
    }

    private static void ProcessHeaders(ApiRequestRequest request, ApiRequest data)
    {
        data.Headers = request.Headers;

        // Find real caller IP.
        if (data.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            data.ForwardedIp = forwardedFor;
            data.Headers.Remove("X-Forwarded-For");
        }

        // If request contains cookie, remove sensitive data.
        if (data.Headers.TryGetValue("Cookie", out var cookieHeader))
        {
            data.Headers["Cookie"] = string.Join(
                "; ",
                cookieHeader
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(c => c.StartsWith(".AspNetCore.Cookies=") ? $".AspNetCore.Cookies=<Removed>" : c)
            );
        }

        if (data.Headers.ContainsKey("Authorization"))
            data.Headers["Authorization"] = "<Removed>";

        if (data.Headers.ContainsKey("ApiKey"))
            data.Headers["ApiKey"] = "<Removed>";

        // Remove useless headers
        data.Headers.Remove("sec-ch-ua");
        data.Headers.Remove("sec-ch-ua-mobile");
        data.Headers.Remove("Sec-Fetch-Dest");
        data.Headers.Remove("Sec-Fetch-Mode");
        data.Headers.Remove("Sec-Fetch-Site");

        if (data.Headers.Count == 0)
            data.Headers = null;
    }

    private static void ProcessParameters(ApiRequestRequest request, ApiRequest data)
    {
        if (request.Parameters.Count > 0)
            data.Parameters = request.Parameters;
    }
}
