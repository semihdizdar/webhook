using Application.DTOs;
using Application.Interfaces;
using Domain.Events;

namespace Infrastructure.Queue;

public class RetryQueuePublisher : IRetryQueuePublisher
{
    private readonly IQueuePublisher _queuePublisher;

    public RetryQueuePublisher(IQueuePublisher queuePublisher)
    {
        _queuePublisher = queuePublisher;
    }

    public async Task EnqueueFailedEndpointsAsync(WebhookReceivedEvent dto, IEnumerable<string> failedEndpoints)
    {
        foreach (var url in failedEndpoints)
        {
            var retryDto = new
            {
                dto.Id,
                Data = dto.Data,
                Endpoint = url
            };
            await _queuePublisher.PublishAsync(retryDto, "retry-webhooks");
        }
    }
}