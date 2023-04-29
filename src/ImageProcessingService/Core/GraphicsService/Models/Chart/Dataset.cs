namespace ImageProcessingService.Core.GraphicsService.Models.Chart;

public class Dataset
{
    public string Label { get; set; } = null!;
    public List<DataPoint> Data { get; set; } = new();
    public string? Color { get; set; }
    public int Width { get; set; }

    public IEnumerable<object> GetAtomicValues()
    {
        yield return Label;

        foreach (var value in Data.SelectMany(point => point.GetAtomicValues()))
            yield return value;

        if (!string.IsNullOrEmpty(Color))
            yield return Color;

        yield return Width;
    }
}
