using Application.DTOs;
using Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Domain.Events;

namespace Infrastructure.Queue;

public class WebhookQueuePublisher : IQueuePublisher
{
    private readonly IConnection _connection;

    public WebhookQueuePublisher(IConnection connection)
    {
        _connection = connection;
    }
    
    public Task PublishAsync(object message, string queueName)
    {
        using var channel = _connection.CreateModel();
        
        channel.QueueDeclare(
            queue: queueName,
            durable: true,     // restart sonrası kaybolmasın
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true; 
        
        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}