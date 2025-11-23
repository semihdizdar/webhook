namespace Domain.Queues;

public interface IQueueConsumer
{
    Task StartAsync(CancellationToken cancellationToken);

}