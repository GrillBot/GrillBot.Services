using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request;

namespace AuditLogService.Processors.Request;

public class ApiRequestProcessor : RequestProcessorBase
{
    public ApiRequestProcessor(DiscordManager discordManager) : base(discordManager)
    {
    }
    
    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        if (entity.Type is not LogType.Api)
            return Task.CompletedTask;

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
