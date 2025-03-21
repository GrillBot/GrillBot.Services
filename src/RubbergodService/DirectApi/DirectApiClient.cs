using System.Text.Json;
using Discord;
using GrillBot.Core.Helpers;
using RubbergodService.DirectApi.Models;

#pragma warning disable IDE0290 // Use primary constructor
namespace RubbergodService.DirectApi;

public class DirectApiClient
{
    private readonly IDiscordClient _discordClient;
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly Dictionary<ulong, ITextChannel> _cachedChannels = [];
    private readonly HashSet<ulong> _authorizedServices;

    public DirectApiClient(IDiscordClient discordClient, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _authorizedServices = configuration.GetRequiredSection("DirectApi").AsEnumerable()
            .Where(o => o.Key.EndsWith(":Id") && !string.IsNullOrEmpty(o.Value))
            .Select(o => Convert.ToUInt64(o.Value))
            .ToHashSet();

        _discordClient = discordClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ApiResponse> SendAsync(ulong channelId, JsonDocument data, int timeout, int timeoutChecks)
    {
        var channel = await GetChannelAsync(channelId);
        var jsonData = await JsonHelper.SerializeJsonDocumentAsync(data);
        var request = await channel.SendMessageAsync($"```json\n{jsonData}\n```");

        return await ReadResponseAsync(timeout, timeoutChecks, request);
    }

    private async Task<ITextChannel> GetChannelAsync(ulong channelId)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_cachedChannels.TryGetValue(channelId, out var cachedChannel))
                return cachedChannel;

            var channel = await _discordClient.GetChannelAsync(channelId)
                ?? throw new ArgumentException($"Unable to find channel with ID {channelId}");

            if (channel is not ITextChannel textChannel)
                throw new ArgumentException("Communication channel is not text channel.");

            _cachedChannels.Add(channelId, textChannel);
            return textChannel;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<ApiResponse> ReadResponseAsync(int timeout, int timeoutChecks, IUserMessage request)
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

        var attachment = response.Attachments.First();
        var httpClient = _httpClientFactory.CreateClient();
        var fileContent = await httpClient.GetByteArrayAsync(attachment.Url);

        return new ApiResponse
        {
            Content = fileContent,
            Filename = attachment.Filename
        };
    }

    private bool IsValidResponse(IMessage? response, IUserMessage request)
    {
        return response is not null &&
            _authorizedServices.Contains(response.Author.Id) &&
            response.Reference is { MessageId.IsSpecified: true } &&
            response.Attachments.Count == 1 &&
            response.Reference.MessageId.Value == request.Id;
    }
}
