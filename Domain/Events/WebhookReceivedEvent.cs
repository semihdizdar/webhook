namespace Domain.Events;

public class WebhookReceivedEvent
{
    public string Id { get; set; }
    public string EventType { get; set; } = "unknown";
    public string Data { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    
    public WebhookReceivedEvent(string eventType, string data)
    {
        EventType = eventType;
        Data = data;
        Id = GenerateDeterministicId(eventType, data);
    }

    private static string GenerateDeterministicId(string eventType, string data)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var raw = $"{eventType}:{data}";
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash);
    }
}