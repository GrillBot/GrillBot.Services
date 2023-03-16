using System.Threading.Channels;

namespace RubbergodService.MemberSynchronization;

public class MemberSyncQueue
{
    private Channel<string> Queue { get; }

    public MemberSyncQueue()
    {
        var options = new BoundedChannelOptions(int.MaxValue)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        Queue = Channel.CreateBounded<string>(options);
    }

    public async ValueTask AddToQueueAsync(string memberId)
    {
        await Queue.Writer.WriteAsync(memberId);
    }

    public async ValueTask<string> DequeueAsync(CancellationToken cancellationToken)
    {
        return await Queue.Reader.ReadAsync(cancellationToken);
    }
}
