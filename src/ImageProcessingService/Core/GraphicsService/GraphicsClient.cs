using GrillBot.Core.Managers.Performance;
using ImageProcessingService.Core.GraphicsService.Models.Chart;
using ImageProcessingService.Core.GraphicsService.Models.Images;

namespace ImageProcessingService.Core.GraphicsService;

public class GraphicsClient : RestServiceBase, IGraphicsClient
{
    public override string ServiceName => "Graphics";

    public GraphicsClient(ICounterManager counterManager, IHttpClientFactory httpClientFactory) : base(counterManager, () => httpClientFactory.CreateClient("Graphics"))
    {
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            await ProcessRequestAsync(
                () =>
                {
                    using var message = new HttpRequestMessage(HttpMethod.Head, "health");
                    return HttpClient.SendAsync(message);
                },
                _ => EmptyResult
            );

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<byte[]> CreateChartAsync(ChartRequestData request)
    {
        return await ProcessRequestAsync(
            () => HttpClient.PostAsJsonAsync("chart", request),
            response => response.Content.ReadAsByteArrayAsync()
        );
    }

    public async Task<byte[]> CreateWithoutAccidentImageAsync(WithoutAccidentRequestData request)
    {
        return await ProcessRequestAsync(
            () => HttpClient.PostAsJsonAsync("image/without-accident", request),
            response => response.Content.ReadAsByteArrayAsync()
        );
    }

    public async Task<byte[]> CreatePointsImageAsync(PointsImageRequest imageRequest)
    {
        return await ProcessRequestAsync(
            () => HttpClient.PostAsJsonAsync("image/points", imageRequest),
            response => response.Content.ReadAsByteArrayAsync()
        );
    }

    public async Task<List<byte[]>> CreatePeepoAngryAsync(List<byte[]> avatarFrames)
    {
        var result = await ProcessRequestAsync(
            () => HttpClient.PostAsJsonAsync("image/peepo/angry", avatarFrames),
            response => response.Content.ReadFromJsonAsync<List<byte[]>>()
        );

        return result!;
    }

    public async Task<List<byte[]>> CreatePeepoLoveAsync(List<byte[]> avatarFrames)
    {
        var result = await ProcessRequestAsync(
            () => HttpClient.PostAsJsonAsync("image/peepo/love", avatarFrames),
            response => response.Content.ReadFromJsonAsync<List<byte[]>>()
        );

        return result!;
    }
}
