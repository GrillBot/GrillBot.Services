using AuditLogService.Core.Enums;
using AuditLogService.Processors.Request;
using AuditLogService.Processors.Request.Abstractions;

#pragma warning disable S3604 // Member initializer values should not be redundant
namespace AuditLogService.Processors;

public sealed class RequestProcessorFactory(IServiceProvider _serviceProvider) : IDisposable
{
    private readonly Dictionary<LogType, RequestProcessorBase> _cachedProcessors = [];

    public RequestProcessorBase Create(LogType type)
    {
        if (_cachedProcessors.TryGetValue(type, out var processor))
            return processor;

        processor = type switch
        {
            LogType.Info or LogType.Warning or LogType.Error => new LogMessageProcessor(_serviceProvider),
            LogType.ChannelCreated => new ChannelCreatedProcessor(_serviceProvider),
            LogType.ChannelDeleted => new ChannelDeletedProcessor(_serviceProvider),
            LogType.ChannelUpdated => new ChannelUpdatedProcessor(_serviceProvider),
            LogType.EmoteDeleted => new DeletedEmoteProcessor(_serviceProvider),
            LogType.OverwriteCreated => new OverwriteCreatedProcessor(_serviceProvider),
            LogType.OverwriteDeleted => new OverwriteDeletedProcessor(_serviceProvider),
            LogType.OverwriteUpdated => new OverwriteUpdatedProcessor(_serviceProvider),
            LogType.Unban => new UnbanProcessor(_serviceProvider),
            LogType.MemberUpdated => new MemberUpdatedProcessor(_serviceProvider),
            LogType.MemberRoleUpdated => new MemberRoleUpdatedProcessor(_serviceProvider),
            LogType.GuildUpdated => new GuildUpdatedProcessor(_serviceProvider),
            LogType.UserLeft => new UserLeftProcessor(_serviceProvider),
            LogType.UserJoined => new UserJoinedProcessor(_serviceProvider),
            LogType.MessageEdited => new MessageEditedProcessor(_serviceProvider),
            LogType.MessageDeleted => new MessageDeletedProcessor(_serviceProvider),
            LogType.InteractionCommand => new InteractionCommandProcessor(_serviceProvider),
            LogType.ThreadDeleted => new ThreadDeletedProcessor(_serviceProvider),
            LogType.JobCompleted => new JobCompletedProcessor(_serviceProvider),
            LogType.Api => new ApiRequestProcessor(_serviceProvider),
            LogType.ThreadUpdated => new ThreadUpdatedProcessor(_serviceProvider),
            LogType.RoleDeleted => new RoleDeletedProcessor(_serviceProvider),
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
