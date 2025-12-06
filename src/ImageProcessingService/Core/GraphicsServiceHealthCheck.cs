using Graphics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ImageProcessingService.Core;

public class GraphicsServiceHealthCheck(IGraphicsClient graphicsClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            await graphicsClient.IsHealthyAsync(cancellationToken);
            return new HealthCheckResult(HealthStatus.Healthy);
        }
        catch (Exception)
        {
            return new HealthCheckResult(HealthStatus.Unhealthy);
        }
    }
}
