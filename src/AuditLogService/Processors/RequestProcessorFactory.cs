using AuditLogService.Core.Enums;
using AuditLogService.Processors.Request;
using AuditLogService.Processors.Request.Abstractions;

namespace AuditLogService.Processors;

public sealed class RequestProcessorFactory : IDisposable
{
    private IServiceProvider ServiceProvider { get; }

    private readonly Dictionary<LogType, RequestProcessorBase> _cachedProcessors = new();

    public RequestProcessorFactory(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public RequestProcessorBase Create(LogType type)
    {
        if (_cachedProcessors.TryGetValue(type, out var processor))
            return processor;

        processor = type switch
        {
            LogType.Info or LogType.Warning or LogType.Error => new LogMessageProcessor(ServiceProvider),
            LogType.ChannelCreated => new ChannelCreatedProcessor(ServiceProvider),
            LogType.ChannelDeleted => new ChannelDeletedProcessor(ServiceProvider),
            LogType.ChannelUpdated => new ChannelUpdatedProcessor(ServiceProvider),
            LogType.EmoteDeleted => new DeletedEmoteProcessor(ServiceProvider),
            LogType.OverwriteCreated => new OverwriteCreatedProcessor(ServiceProvider),
            LogType.OverwriteDeleted => new OverwriteDeletedProcessor(ServiceProvider),
            LogType.OverwriteUpdated => new OverwriteUpdatedProcessor(ServiceProvider),
            LogType.Unban => new UnbanProcessor(ServiceProvider),
            LogType.MemberUpdated => new MemberUpdatedProcessor(ServiceProvider),
            LogType.MemberRoleUpdated => new MemberRoleUpdatedProcessor(ServiceProvider),
            LogType.GuildUpdated => new GuildUpdatedProcessor(ServiceProvider),
            LogType.UserLeft => new UserLeftProcessor(ServiceProvider),
            LogType.UserJoined => new UserJoinedProcessor(ServiceProvider),
            LogType.MessageEdited => new MessageEditedProcessor(ServiceProvider),
            LogType.MessageDeleted => new MessageDeletedProcessor(ServiceProvider),
            LogType.InteractionCommand => new InteractionCommandProcessor(ServiceProvider),
            LogType.ThreadDeleted => new ThreadDeletedProcessor(ServiceProvider),
            LogType.JobCompleted => new JobCompletedProcessor(ServiceProvider),
            LogType.Api => new ApiRequestProcessor(ServiceProvider),
            LogType.ThreadUpdated => new ThreadUpdatedProcessor(ServiceProvider),
            LogType.RoleDeleted => new RoleDeletedProcessor(ServiceProvider),
            _ => throw new NotSupportedException($"Unsupported type {type}")
        };

        _cachedProcessors.Add(type, processor);
        return processor;
    }

    public void Dispose()
    {
        _cachedProcessors.Clear();
    }
}
