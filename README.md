# Event-Driven Webhook Pipeline

A **production-ready, resilient webhook processing system** built with **.NET 8** and **RabbitMQ**.  
Supports multiple endpoints, idempotency, retry queues, and dead-letter queues (DLQ) for reliable message delivery.

---

## Features

- Event-driven architecture with RabbitMQ  
- Reliable delivery to multiple endpoints  
- **Idempotency** to prevent duplicate processing  
- **Retry queues** for failed endpoint deliveries  
- **Dead-letter queue (DLQ)** for messages exceeding max retries  
- Fully asynchronous **Publisher → Consumer → Dispatcher pipeline
---
