using Domain.Events;

namespace Application.Interfaces;

public interface IRetryQueuePublisher
{
    /// <summary>
    /// Başarısız endpoint mesajlarını retry kuyruğuna ekler
    /// </summary>
    /// <param name="dto">Gönderilen webhook event</param>
    /// <param name="failedEndpoints">Başarısız giden endpoint listesi</param>
    Task EnqueueFailedEndpointsAsync(WebhookReceivedEvent dto, IEnumerable<string> failedEndpoints);
}