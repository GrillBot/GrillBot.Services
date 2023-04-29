namespace ImageProcessingService.Core.GraphicsService.Models.Chart;

public class ChartOptions
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string BackgroundColor { get; set; } = null!;
    public string Type { get; set; } = "line";
    public string LegendPosition { get; set; } = null!;
    public int? PointsRadius { get; set; }

    public IEnumerable<object> GetAtomicValues()
    {
        yield return Width;
        yield return Height;
        yield return BackgroundColor;
        yield return Type;
        yield return LegendPosition;

        if (PointsRadius is not null)
            yield return PointsRadius;
    }
}
