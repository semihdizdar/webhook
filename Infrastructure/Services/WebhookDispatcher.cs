using Application.DTOs;
using Application.Interfaces;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using Application.DTOs;
using Application.Interfaces;
using Domain.Events;

namespace Infrastructure.Services;

public class WebhookDispatcher : IWebhookDispatcher
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncRetryPolicy _retryPolicy;

    public WebhookDispatcher(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        // Retry policy: 3 kez, exponential backoff
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }

    public async Task<IEnumerable<string>> DispatchAsync(WebhookReceivedEvent webhookReceivedEvent, IEnumerable<string> endpoints)
    {
        var client = _httpClientFactory.CreateClient();
        var failedEndpoints = new List<string>();

        foreach (var url in endpoints)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await client.PostAsJsonAsync(url, webhookReceivedEvent.Data);
                    response.EnsureSuccessStatusCode();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dispatcher] Failed to send webhook to {url}: {ex.Message}");
                failedEndpoints.Add(url);
                //Burada sadece url atıyoruz version 1.2 ile birlikte url + data bırakılacak queueu bunu tüketecek bir
                //consumer ile tekrar gönderme denemesi yapılacak
            }
        }

        return failedEndpoints;
    }
}