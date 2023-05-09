using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request.Abstractions;
using Discord;
using Discord.Rest;
using GrillBot.Core.Extensions;
using EmbedField = AuditLogService.Core.Entity.EmbedField;

namespace AuditLogService.Processors.Request;

public class MessageDeletedProcessor : RequestProcessorBase
{
    public MessageDeletedProcessor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

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

    private static EmbedInfo? ConvertEmbed(string json)
    {
        if (!EmbedBuilder.TryParse(json, out var embedBuilder))
            return null;

        var embed = embedBuilder.Build();
        var providerName = embed.Provider?.Name;
        var embedEntity = new EmbedInfo
        {
            Id = Guid.NewGuid(),
            Title = embed.Title,
            Type = embed.Type.ToString(),
            AuthorName = embed.Author?.Name,
            ContainsFooter = embed.Footer is not null,
            VideoInfo = ParseVideoInfo(embed.Video, providerName),
            ProviderName = providerName
        };

        if (embed.Image is not null)
            embedEntity.ImageInfo = $"{embed.Image.Value.Url} ({embed.Image.Value.Width}x{embed.Image.Value.Height})";

        if (embed.Thumbnail is not null)
            embedEntity.ThumbnailInfo = $"{embed.Thumbnail.Value.Url} ({embed.Thumbnail.Value.Width}x{embed.Thumbnail.Value.Height})";

        foreach (var field in embed.Fields)
        {
            embedEntity.Fields.Add(new EmbedField
            {
                Id = Guid.NewGuid(),
                Name = field.Name,
                Value = field.Value,
                Inline = field.Inline
            });
        }

        return embedEntity;
    }

    private static string? ParseVideoInfo(EmbedVideo? video, string? providerName)
    {
        if (video == null)
            return null;

        var size = $"({video.Value.Width}x{video.Value.Height})";

        if (providerName != "Twitch")
            return $"{video.Value.Url} {size}";

        var url = new Uri(video.Value.Url);
        var queryFields = url.Query[1..].Split('&').Select(o => o.Split('=')).ToDictionary(o => o[0], o => o[1]);
        return $"{queryFields["channel"]} {size}";
    }
}
