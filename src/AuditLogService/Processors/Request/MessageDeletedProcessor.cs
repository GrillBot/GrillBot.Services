using AuditLogService.Core.Entity;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using EmbedField = AuditLogService.Core.Entity.EmbedField;

namespace AuditLogService.Processors.Request;

public class MessageDeletedProcessor(IServiceProvider serviceProvider) : RequestProcessorBase(serviceProvider)
{
    public override async Task ProcessAsync(LogItem entity, LogRequest request)
    {
        var auditLog = await FindAuditLogAsync(request);

        if (auditLog is not null)
        {
            entity.DiscordId = auditLog.Id.ToString();
            entity.CreatedAt = auditLog.CreatedAt.UtcDateTime;
            entity.UserId = auditLog.User.Id.ToString();
        }
        else
        {
            entity.UserId = request.MessageDeleted!.AuthorId;
        }

        entity.MessageDeleted = new MessageDeleted
        {
            AuthorId = request.MessageDeleted!.AuthorId,
            Content = request.MessageDeleted.Content,
            MessageCreatedAt = request.MessageDeleted.MessageCreatedAt
        };

        foreach (var embedEntity in request.MessageDeleted.Embeds.Select(ConvertEmbed).Where(embedEntity => embedEntity is not null))
            entity.MessageDeleted.Embeds.Add(embedEntity!);
    }

    protected override bool IsValidAuditLogItem(IAuditLogEntry entry, LogRequest request)
    {
        var timeLimit = DateTime.UtcNow.AddMinutes(-1);
        var data = (MessageDeleteAuditLogData)entry.Data;

        return entry.CreatedAt.UtcDateTime >= timeLimit && data.Target.Id == request.MessageDeleted!.AuthorId.ToUlong() && data.ChannelId == request.ChannelId!.ToUlong();
    }

    private static EmbedInfo? ConvertEmbed(EmbedRequest request)
    {
        var embedEntity = new EmbedInfo
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Type = request.Type,
            AuthorName = request.AuthorName,
            ContainsFooter = request.ContainsFooter,
            VideoInfo = request.VideoInfo,
            ProviderName = request.ProviderName,
            ImageInfo = request.ImageInfo,
            ThumbnailInfo = request.ThumbnailInfo
        };

        foreach (var field in request.Fields)
        {
            embedEntity.Fields.Add(new EmbedField
            {
                Id = Guid.NewGuid(),
                Name = field.Name,
                Value = field.Value.ToString()!,
                Inline = field.IsInline
            });
        }

        return embedEntity;
    }
}
