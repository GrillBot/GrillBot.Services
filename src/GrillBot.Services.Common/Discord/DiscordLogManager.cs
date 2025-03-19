using Discord;
using Discord.Rest;
using Microsoft.Extensions.Logging;

namespace GrillBot.Services.Common.Discord;

public class DiscordLogManager
{
    private readonly ILoggerFactory _loggerFactory;

    public DiscordLogManager(IDiscordClient client, ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        ((DiscordRestClient)client).Log += OnLogAsync;
    }

    private Task OnLogAsync(LogMessage message)
    {
        var logger = _loggerFactory.CreateLogger(message.Source);

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
