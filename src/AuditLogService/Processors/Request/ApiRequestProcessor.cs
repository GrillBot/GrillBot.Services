using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class ApiRequestProcessor : RequestProcessorBase
{
    public ApiRequestProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
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
            ApiGroupName = apiRequest.ApiGroupName
        };

        return Task.CompletedTask;
    }
}
