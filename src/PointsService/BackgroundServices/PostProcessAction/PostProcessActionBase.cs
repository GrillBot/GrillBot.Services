using PointsService.Core.Repository;

namespace PointsService.BackgroundServices.PostProcessAction;

public abstract class PostProcessActionBase
{
    protected PointsServiceRepository Repository { get; }
    private List<object> Parameters { get; }

    protected PostProcessActionBase(PointsServiceRepository repository)
    {
        Repository = repository;
        Parameters = new List<object>();
    }

    public abstract Task ProcessAsync();

    public PostProcessActionBase SetParameters(params object[] parameters)
    {
        Parameters.Clear();
        Parameters.AddRange(parameters);
        return this;
    }

    protected TParameter? GetParameter<TParameter>()
        => Parameters.OfType<TParameter>().FirstOrDefault();
}
