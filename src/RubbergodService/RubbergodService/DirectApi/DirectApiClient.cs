using System.Text.Json;
using Discord;
using RubbergodService.Core.Helpers;

namespace RubbergodService.DirectApi;

public class DirectApiClient
{
    private IDiscordClient DiscordClient { get; }

    private Dictionary<ulong, ITextChannel> CachedChannels { get; } = new();
    private HashSet<ulong> AuthorizedServices { get; }
    private SemaphoreSlim Semaphore { get; }
    private HttpClient HttpClient { get; }

    public DirectApiClient(IDiscordClient discordClient, IConfiguration configuration)
    {
        Semaphore = new SemaphoreSlim(1);
        DiscordClient = discordClient;
        HttpClient = new HttpClient();

        AuthorizedServices = configuration.GetRequiredSection("DirectApi").AsEnumerable()
            .Where(o => o.Key.EndsWith(":Id") && !string.IsNullOrEmpty(o.Value))
            .Select(o => Convert.ToUInt64(o.Value))
            .ToHashSet();
    }

    public async Task<string> SendAsync(ulong channelId, JsonDocument data, int timeout, int timeoutChecks)
    {
        var channel = await GetChannelAsync(channelId);
        var jsonData = await JsonHelper.SerializeJsonDocumentAsync(data);
        var request = await channel.SendMessageAsync($"```json\n{jsonData}\n```");

        return await ReadResponseAsync(timeout, timeoutChecks, request);
    }

    private async Task<ITextChannel> GetChannelAsync(ulong channelId)
    {
        await Semaphore.WaitAsync();
        try
        {
            if (CachedChannels.TryGetValue(channelId, out var cachedChannel)) return cachedChannel;

            var channel = await DiscordClient.GetChannelAsync(channelId);
            if (channel is null)
                throw new ArgumentException($"Unable to find channel with ID {channelId}");
            if (channel is not ITextChannel textChannel)
                throw new ArgumentException("Communication channel is not text channel.");

            CachedChannels.Add(channelId, textChannel);
            return textChannel;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private async Task<string> ReadResponseAsync(int timeout, int timeoutChecks, IUserMessage request)
    {
        var delay = Convert.ToInt32(timeout / timeoutChecks);

        IMessage? response = null;
        for (var i = 0; i < timeoutChecks; i++)
        {
            await Task.Delay(delay);

            var messages = await request.Channel.GetMessagesAsync(request.Id, Direction.After).FlattenAsync();
            response = messages.FirstOrDefault(o => IsValidResponse(o, request));
            if (response is not null) break;
        }

        if (response is null)
            throw new HttpRequestException("Unable to find response message");
        return await HttpClient.GetStringAsync(response.Attachments.First().Url);
    }

    private bool IsValidResponse(IMessage? response, IUserMessage request)
    {
        return response is not null && AuthorizedServices.Contains(response.Author.Id) && response.Reference is { MessageId.IsSpecified: true } &&
               response.Attachments.Count == 1 && response.Reference.MessageId.Value == request.Id;
    }
}
