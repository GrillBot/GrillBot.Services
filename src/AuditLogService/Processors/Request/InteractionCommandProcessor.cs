using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors.Request;

public class InteractionCommandProcessor : RequestProcessorBase
{
    public InteractionCommandProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        entity.InteractionCommand = new InteractionCommand
        {
            Duration = request.InteractionCommand!.Duration,
            Exception = request.InteractionCommand.Exception,
            Locale = request.InteractionCommand.Locale,
            Name = request.InteractionCommand.Name,
            Parameters = request.InteractionCommand.Parameters.ConvertAll(o => new InteractionCommandParameter
            {
                Name = o.Name,
                Type = o.Type,
                Value = o.Value
            }),
            CommandError = request.InteractionCommand.CommandError,
            ErrorReason = request.InteractionCommand.ErrorReason,
            HasResponded = request.InteractionCommand.HasResponded,
            IsSuccess = request.InteractionCommand.IsSuccess,
            MethodName = request.InteractionCommand.MethodName,
            ModuleName = request.InteractionCommand.ModuleName,
            IsValidToken = request.InteractionCommand.IsValidToken
        };

        return Task.CompletedTask;
    }
}
