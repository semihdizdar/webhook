using Application.Interfaces;

namespace Infrastructure.Services;

public class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly HashSet<string> _processed = new();

    public Task<bool> ExistsAsync(string id)
    {
        lock (_processed) return Task.FromResult(_processed.Contains(id));
    }

    public Task MarkAsProcessedAsync(string id)
    {
        lock (_processed) _processed.Add(id);
        return Task.CompletedTask;
    }
}