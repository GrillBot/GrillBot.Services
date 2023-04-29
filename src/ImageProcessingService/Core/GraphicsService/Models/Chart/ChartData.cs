namespace ImageProcessingService.Core.GraphicsService.Models.Chart;

public class ChartData
{
    public Label? TopLabel { get; set; }
    public List<Dataset> Datasets { get; set; } = new();

    public IEnumerable<object> GetAtomicValues()
    {
        if (TopLabel is not null)
        {
            foreach (var value in TopLabel.GetAtomicValues())
                yield return value;
        }

        foreach (var value in Datasets.SelectMany(dataset => dataset.GetAtomicValues()))
            yield return value;
    }
}
