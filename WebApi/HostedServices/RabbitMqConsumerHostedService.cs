using Domain.Queues;

namespace WebApi.HostedServices;

public class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly IQueueConsumer _consumer;

    public RabbitMqConsumerHostedService(IQueueConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartAsync(stoppingToken);
    }
}