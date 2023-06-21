using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ImageProcessingService.Core.GraphicsService;

public class GraphicsServiceHealthCheck : IHealthCheck
{
    private IGraphicsClient GraphicsClient { get; }

    public GraphicsServiceHealthCheck(IGraphicsClient graphicsClient)
    {
        GraphicsClient = graphicsClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        var isAvailable = await GraphicsClient.IsAvailableAsync();
        return new HealthCheckResult(isAvailable ? HealthStatus.Healthy : HealthStatus.Unhealthy);
    }
}
