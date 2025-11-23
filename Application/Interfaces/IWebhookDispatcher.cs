using Application.DTOs;
using Domain.Events;

namespace Application.Interfaces;

public interface IWebhookDispatcher
{
    Task<IEnumerable<string>> DispatchAsync(WebhookReceivedEvent dto, IEnumerable<string> endpoints);

}