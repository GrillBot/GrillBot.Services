using Discord;
using Discord.Rest;

namespace AuditLogService.Core.Discord;

public class DiscordLogManager
{
    private DiscordRestClient Client { get; }
    private ILoggerFactory LoggerFactory { get; }

    public DiscordLogManager(IDiscordClient client, ILoggerFactory loggerFactory)
    {
        Client = (DiscordRestClient)client;
        LoggerFactory = loggerFactory;
        Client.Log += OnLogAsync;
    }

    private Task OnLogAsync(LogMessage message)
    {
        var logger = LoggerFactory.CreateLogger(message.Source);

        switch (message.Severity)
        {
            case LogSeverity.Critical:
                logger.LogCritical(message.Exception, "{Message}", message.Message);
                break;
            case LogSeverity.Error:
                logger.LogError(message.Exception, "{Message}", message.Message);
                break;
            case LogSeverity.Warning:
                if (message.Exception == null) logger.LogWarning("{Message}", message.Message);
                else logger.LogWarning(message.Exception, "{Message}", message.Message);
                break;
            case LogSeverity.Info:
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                logger.LogInformation("{Message}", message.Message);
                break;
        }

        return Task.CompletedTask;
    }
}
