namespace Application.DTOs;

public class WebhookRequestDto
{
    public string EventType { get; set; } = "unknown";
    public string Data { get; set; } = string.Empty; 
    
}