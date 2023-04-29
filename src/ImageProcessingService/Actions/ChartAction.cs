using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using ImageProcessingService.Core.GraphicsService;
using ImageProcessingService.Core.GraphicsService.Models.Chart;
using ImageProcessingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Actions;

public class ChartAction : ApiActionBase
{
    private IGraphicsClient GraphicsClient { get; }

    public ChartAction(IGraphicsClient graphicsClient)
    {
        GraphicsClient = graphicsClient;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (ChartRequest)Parameters[0]!;
        var images = new List<MagickImage>();

        try
        {
            foreach (var chartRequest in request.Requests)
            {
                var image = await GraphicsClient.CreateChartAsync(chartRequest);
                images.Add(new MagickImage(image, MagickFormat.Png));
            }

            var mergedChart = MergeChartsAndGetData(images, request.Requests);
            return new ApiResult(StatusCodes.Status200OK, new FileContentResult(mergedChart, "image/png"));
        }
        finally
        {
            foreach (var image in images)
                image.Dispose();
        }
    }

    private static byte[] MergeChartsAndGetData(IReadOnlyList<MagickImage> charts, IReadOnlyList<ChartRequestData> requests)
    {
        var finalHeight = requests.Sum(o => o.Options.Height);
        var width = requests.Max(o => o.Options.Width);
        var background = requests[0].Options.BackgroundColor;

        using var resultImage = new MagickImage(new MagickColor(background), width, finalHeight);

        IDrawables<byte> drawables = new Drawables();
        for (var i = 0; i < charts.Count; i++)
        {
            var top = requests.Take(i).Sum(o => o.Options.Height);
            drawables = drawables
                .Composite(0, top, CompositeOperator.Multiply, charts[i]);

            if (i > 0)
                drawables = drawables.Line(0, top, width, top);
        }

        drawables.Draw(resultImage);
        return resultImage.ToByteArray(MagickFormat.Png);
    }
}
