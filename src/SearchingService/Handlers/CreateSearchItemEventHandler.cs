using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Options;
using SearchingService.Core.Entity;
using SearchingService.Models.Events;
using SearchingService.Options;

namespace SearchingService.Handlers;

public class CreateSearchItemEventHandler : BaseEventHandlerWithDb<SearchItemPayload, SearchingServiceContext>
{
    private readonly AppOptions _options;

    public CreateSearchItemEventHandler(ILoggerFactory loggerFactory, SearchingServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher, IOptions<AppOptions> options) : base(loggerFactory, dbContext, counterManager, publisher)
    {
        _options = options.Value;
    }

    protected override async Task HandleInternalAsync(SearchItemPayload payload, Dictionary<string, string> headers)
    {
        var created = DateTime.UtcNow;
        var entity = new SearchItem
        {
            ChannelId = payload.ChannelId,
            Content = payload.Content,
            CreatedAt = created,
            GuildId = payload.GuildId,
            UserId = payload.UserId,
            ValidTo = payload.ValidToUtc ?? DateTime.UtcNow.Add(_options.DefaultItemValidity)
        };

        await DbContext.AddAsync(entity);
        await ContextHelper.SaveChagesAsync();
    }
}
