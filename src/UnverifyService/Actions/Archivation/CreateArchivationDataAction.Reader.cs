namespace UnverifyService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    public async Task<int> CountAsync()
    {
        var query = ContextHelper.DbContext.LogItems.Where(o => o.CreatedAt <= ExpirationDate);
        return await ContextHelper.ReadCountAsync(query, CancellationToken);
    }
}
