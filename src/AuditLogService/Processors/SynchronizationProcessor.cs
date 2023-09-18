namespace AuditLogService.Processors;

public class SynchronizationProcessor
{
    private SemaphoreSlim Semaphore { get; }

    public SynchronizationProcessor()
    {
        Semaphore = new SemaphoreSlim(1);
    }

    public async Task RunSynchronizedActionAsync(Func<Task> asyncAction)
    {
        await Semaphore.WaitAsync();

        try
        {
            await asyncAction();
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
