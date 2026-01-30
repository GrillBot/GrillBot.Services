using GrillBot.Core.RabbitMQ.V2.Publisher;
using AuditLog.Enums;
using AuditLog.Models.Events.Create;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace GrillBot.Services.Common.Infrastructure.Api.Filters;

public class ExceptionFilter(
    IRabbitPublisher _rabbitPublisher,
    ILogger<ExceptionFilter> _logger
) : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
        var controllerName = descriptor.ControllerTypeInfo.Name;
        var actionName = descriptor.MethodInfo.Name;

        var logRequest = new LogRequest(LogType.Error, DateTime.UtcNow)
        {
            LogMessage = new LogMessageRequest
            {
                Message = CreateErrorMessage(context),
                SourceAppName = Assembly.GetEntryAssembly()!.GetName().Name!,
                Source = $"{controllerName}.{actionName}"
            }
        };

        try
        {
            await _rabbitPublisher.PublishAsync(new CreateItemsMessage(logRequest));
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Cannot publish log message to RabbitMQ.");
        }
    }

    private static string CreateErrorMessage(ExceptionContext context)
    {
        var queryString = context.HttpContext.Request.QueryString;
        var fields = new List<string>
        {
            $"Path: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}",
            $"QueryParams: {(queryString.HasValue ? queryString.ToUriComponent() : "-")}",
            $"Exception: {context.Exception}"
        };

        return string.Join("\n", fields);
    }
}
