﻿using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Suggestions;

public class EmoteSuggestionRequestPayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "EmoteSuggestionRequests";

    public string Name { get; set; } = null!;
    public string ReasonToAdd { get; set; } = null!;
    public byte[] Image { get; set; } = null!;
    public ulong GuildId { get; set; }
    public ulong FromUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsAnimated { get; set; }
    public string Locale { get; set; } = null!;

    public EmoteSuggestionRequestPayload()
    {
    }

    public EmoteSuggestionRequestPayload(
        string name,
        string reasonToAdd,
        byte[] image,
        ulong guildId,
        ulong fromUserId,
        DateTime createdAtUtc,
        bool isAnimated,
        string locale
    )
    {
        Name = name;
        ReasonToAdd = reasonToAdd;
        Image = image;
        GuildId = guildId;
        FromUserId = fromUserId;
        CreatedAtUtc = createdAtUtc;
        IsAnimated = isAnimated;
        Locale = locale;
    }

    public static EmoteSuggestionRequestPayload Create(
        string name,
        string reasonToAdd,
        byte[] image,
        ulong guildId,
        ulong fromUserId,
        bool isAnimated,
        string locale
    ) => new(name, reasonToAdd, image, guildId, fromUserId, DateTime.UtcNow, isAnimated, locale);

    public static EmoteSuggestionRequestPayload Create(string name, string reasonToAdd, byte[] image, IGuild guild, IUser user, bool isAnimated, string locale)
        => Create(name, reasonToAdd, image, guild.Id, user.Id, isAnimated, locale);
}
