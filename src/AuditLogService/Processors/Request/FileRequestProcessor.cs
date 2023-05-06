using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using File = AuditLogService.Core.Entity.File;

namespace AuditLogService.Processors.Request;

public class FileRequestProcessor : RequestProcessorBase
{
    public FileRequestProcessor(DiscordManager discordManager) : base(discordManager)
    {
    }

    public override Task ProcessAsync(LogItem entity, LogRequest request)
    {
        foreach (var file in request.Files.Select(ConvertRequestToEntity))
            entity.Files.Add(file);

        return Task.CompletedTask;
    }

    private static File ConvertRequestToEntity(FileRequest request)
    {
        return new File
        {
            Extension = request.Extension,
            Id = Guid.NewGuid(),
            Filename = request.Filename,
            Size = request.Size
        };
    }
}
