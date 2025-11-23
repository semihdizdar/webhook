namespace Application.Interfaces;

public interface IIdempotencyStore
{
    Task<bool> ExistsAsync(string id);
    Task MarkAsProcessedAsync(string id);
}