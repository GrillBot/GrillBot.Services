using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using GrillBot.Core.Services.Graphics;
using ImageProcessingService.Models;
using Microsoft.AspNetCore.Mvc;
using GrillBot.Core.Services.Graphics.Models.Chart;
using ImageMagick.Drawing;

namespace ImageProcessingService.Actions;

public class ChartAction(IGraphicsClient _graphicsClient) : ApiActionBase
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (ChartRequest)Parameters[0]!;

        var imageTasks = request.Requests.ConvertAll(_graphicsClient.CreateChartAsync);
        var imagesData = await Task.WhenAll(imageTasks);
        var images = imagesData.Select(img => new MagickImage(img, MagickFormat.Jpg)).AsParallel().ToList();

        try
        {
            var mergedChart = MergeChartsAndGetData(images, request.Requests);
            return ApiResult.Ok(new FileContentResult(mergedChart, "image/jpeg"));
        }
        finally
        {
            Parallel.ForEach(images, img => img.Dispose());
        }
    }

    private static byte[] MergeChartsAndGetData(IReadOnlyList<MagickImage> charts, IReadOnlyList<ChartRequestData> requests)
    {
        var finalHeight = (uint)requests.Sum(o => o.Options.Height);
        var width = (uint)requests.Max(o => o.Options.Width);
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
        return resultImage.ToByteArray(MagickFormat.Jpg);
    }
}
