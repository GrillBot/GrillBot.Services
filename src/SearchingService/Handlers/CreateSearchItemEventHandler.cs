using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Options;
using SearchingService.Core.Entity;
using SearchingService.Models.Events;
using SearchingService.Options;

namespace SearchingService.Handlers;

public class CreateSearchItemEventHandler(
    IServiceProvider serviceProvider,
    IOptions<AppOptions> _options
) : BaseEventHandlerWithDb<SearchItemPayload, SearchingServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(SearchItemPayload payload, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var created = DateTime.UtcNow;
        var entity = new SearchItem
        {
            ChannelId = payload.ChannelId,
            Content = payload.Content,
            CreatedAt = created,
            GuildId = payload.GuildId,
            UserId = payload.UserId,
            ValidTo = payload.ValidToUtc ?? DateTime.UtcNow.Add(_options.Value.DefaultItemValidity)
        };

        await DbContext.AddAsync(entity);
        await ContextHelper.SaveChagesAsync();
        return RabbitConsumptionResult.Success;
    }
}
