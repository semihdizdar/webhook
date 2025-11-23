using Application.DTOs;
using Domain.Events;

namespace Application.Interfaces;

public interface IQueuePublisher
{
    Task PublishAsync(object message, string queueName);
}