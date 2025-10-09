using GrillBot.Core.RabbitMQ.V2.Publisher;
using AuditLog.Enums;
using AuditLog.Models.Events.Create;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace GrillBot.Services.Common.Infrastructure.Api.Filters;

public class ExceptionFilter(
    IRabbitPublisher _rabbitPublisher
) : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
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

        return _rabbitPublisher.PublishAsync(new CreateItemsMessage(logRequest));
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
