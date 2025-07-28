using GrillBot.Core.RabbitMQ.V2.Messages;

namespace UnverifyService.Models.Events;

public class LogBulkDeleteMessage : IRabbitMessage
{
    public string Topic => "Unverify";
    public string Queue => "LogBulkDelete";

    public List<Guid> Ids { get; set; } = [];

    public LogBulkDeleteMessage()
    {
    }

    public LogBulkDeleteMessage(IEnumerable<Guid> ids)
    {
        Ids = [.. ids];
    }

    public LogBulkDeleteMessage(Guid id)
    {
        Ids = [id];
    }
}
