using Application.Interfaces;
using Domain.Queues;
using Infrastructure.Queue;
using Infrastructure.Services;
using RabbitMQ.Client;
using WebApi.HostedServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// RabbitMQ connection
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new RabbitMQ.Client.ConnectionFactory
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest",
        Port = 5672
    };

    return factory.CreateConnection();
});

// IQueuePublisher
builder.Services.AddSingleton<IQueuePublisher, WebhookQueuePublisher>();
builder.Services.AddSingleton<IQueueConsumer, RabbitMqConsumer>();
builder.Services.AddHostedService<RabbitMqConsumerHostedService>();
builder.Services.AddSingleton<IRetryQueuePublisher, RetryQueuePublisher>();
// Dispatcher + HttpClient
builder.Services.AddHttpClient();
// builder.Services.AddSingleton<IWebhookDispatcher>(sp =>
// {
//     var endpoints = new[] { "https://endpoint1.com/webhook", "https://endpoint2.com/webhook" };
//     return new WebhookDispatcher(sp.GetRequiredService<IHttpClientFactory>(), endpoints);
// });

builder.Services.AddSingleton<IWebhookDispatcher>(sp =>
{
    return new WebhookDispatcher(sp.GetRequiredService<IHttpClientFactory>());
});

// Idempotency store
builder.Services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();