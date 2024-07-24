﻿using GrillBot.Core.RabbitMQ;

namespace SearchingService.Models.Events;

public class SearchItemPayload : IPayload
{
    public string QueueName => "searching:create";

    public string UserId { get; set; } = null!;
    public string GuildId { get; set; } = null!;
    public string ChannelId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime? ValidToUtc { get; set; }
}