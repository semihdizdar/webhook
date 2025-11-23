using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{

    private readonly IQueuePublisher _queuePublisher;
    public WebhookController(IQueuePublisher queuePublisher)
    {
        _queuePublisher = queuePublisher;
    }

    [HttpPost]
    public async Task<IActionResult> Receive([FromBody] WebhookRequestDto dto, CancellationToken cancellationToken)
    {
        var evt = new WebhookReceivedEvent(
            dto.EventType,
            dto.Data
        );

        await _queuePublisher.PublishAsync(evt,"webhooks");

        return Ok(new { status = "queued", id = evt.Id });    }
}