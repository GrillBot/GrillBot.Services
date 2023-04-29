using ImageProcessingService.Core.GraphicsService.Models.Chart;
using ImageProcessingService.Core.GraphicsService.Models.Images;

namespace ImageProcessingService.Core.GraphicsService;

public interface IGraphicsClient
{
    Task<byte[]> CreateChartAsync(ChartRequestData request);
    Task<byte[]> CreateWithoutAccidentImageAsync(WithoutAccidentRequestData request);
    Task<byte[]> CreatePointsImageAsync(PointsImageRequest imageRequest);
    Task<List<byte[]>> CreatePeepoAngryAsync(List<byte[]> avatarFrames);
    Task<List<byte[]>> CreatePeepoLoveAsync(List<byte[]> avatarFrames);
}
