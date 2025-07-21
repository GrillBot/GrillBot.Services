using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using MessageService.Core.Entity;
using MessageService.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Handlers.MessageReceived;

public partial class MessageReceivedHandler(IServiceProvider serviceProvider) : BaseEventHandlerWithDb<MessageReceivedPayload, MessageContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        MessageReceivedPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var channelQuery = DbContext.GuildChannels.AsNoTracking()
            .Where(o => o.GuildId == message.GuildId && o.ChannelId == message.ChannelId && !o.IsDeleted);
        var channel = (await ContextHelper.ReadFirstOrDefaultEntityAsync(channelQuery, cancellationToken)) ?? new();

        if (!channel.IsPointsDisabled)
            await ProcessPointsTransactionRequestAsync(message);

        if (!channel.IsAutoReplyDisabled)
            await ProcessAutoReplyAsync(message);

        return RabbitConsumptionResult.Success;
    }
}
