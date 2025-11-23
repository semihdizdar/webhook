using System.Text;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Domain.Events;
using Domain.Queues;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Queue;

public class RabbitMqConsumer : IQueueConsumer
{
    private readonly IConnection _connection;
    private readonly IWebhookDispatcher _dispatcher;
    private readonly IIdempotencyStore _idempotencyStore;
    private readonly string _queueName = "webhooks";
    private readonly IEnumerable<string> _endpoints;
    private readonly IRetryQueuePublisher _retryQueuePublisher;


    public RabbitMqConsumer(
        IConnection connection,
        IWebhookDispatcher dispatcher,
        IIdempotencyStore idempotencyStore,
        IEnumerable<string> endpoints, IRetryQueuePublisher retryQueuePublisher)
    {
        _connection = connection;
        _dispatcher = dispatcher;
        _idempotencyStore = idempotencyStore;
        _endpoints = endpoints;
        _retryQueuePublisher = retryQueuePublisher;
    }

  public Task StartAsync(CancellationToken cancellationToken)
    {
        var channel = _connection.CreateModel();

        // Queue declare
        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (sender, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var dto = JsonSerializer.Deserialize<WebhookReceivedEvent>(json);

                if (dto == null)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                // Idempotency check
                if (await _idempotencyStore.ExistsAsync(dto.Id))
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                var endpointsList = _endpoints?.ToList() ?? new List<string>();
                endpointsList.Add("www.semihdizdar.com");
                endpointsList.Add("www.github.com");
                
                // Dispatch to endpoints ( artık parametre alıyor )
                var failedEndpoints = await _dispatcher.DispatchAsync(dto, endpointsList);

                if (failedEndpoints != null && failedEndpoints.Any())
                {
                    Console.WriteLine($"[Consumer] Failed endpoints: {string.Join(", ", failedEndpoints)}");
                    await _retryQueuePublisher.EnqueueFailedEndpointsAsync(dto, failedEndpoints);
                }

                // Mark as processed
                await _idempotencyStore.MarkAsProcessedAsync(dto.Id);

                // Ack
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consumer] Error: {ex.Message}");
                // Opsiyonel: retry veya dead-letter logic buraya eklenebilir
            }
        };

        channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);

        Console.WriteLine("[Consumer] Listening...");

        // Consumer’ın çalışmasını sürdürmek için Task.Delay
        return Task.Run(async () =>
        {
            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("[Consumer] Shutting down gracefully...");
                channel.Close();
                channel.Dispose();
            }
        });
    }
}