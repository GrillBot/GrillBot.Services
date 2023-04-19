using System.Threading.Channels;
using PointsService.Core.Entity;

namespace PointsService.BackgroundServices;

public class PostProcessingQueue
{
    private Channel<PostProcessRequest> TransactionQueue { get; }

    public PostProcessingQueue()
    {
        var options = new BoundedChannelOptions(int.MaxValue)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        TransactionQueue = System.Threading.Channels.Channel.CreateBounded<PostProcessRequest>(options);
    }

    public async ValueTask SendRequestAsync(Transaction transaction, bool isDelete)
        => await TransactionQueue.Writer.WriteAsync(new PostProcessRequest(transaction, isDelete));

    public async ValueTask<PostProcessRequest> ReadRequestAsync(CancellationToken cancellationToken)
        => await TransactionQueue.Reader.ReadAsync(cancellationToken);
}
