# External Services

Implementations for external service integrations.

## What Goes Here

- **Email Services**: SMTP, SendGrid, etc.
- **Payment Gateways**: Stripe, PayPal, etc.
- **File Storage**: Azure Blob, S3, etc.
- **Caching**: Redis, Memory Cache
- **Message Queues**: RabbitMQ, Azure Service Bus

## Pattern

Define interfaces in **Domain** or **Application**, implement here.

## Example

```csharp
// Application/Services/IEmailService.cs
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

// Infrastructure/Services/SendGridEmailService.cs
public class SendGridEmailService : IEmailService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        // SendGrid implementation
    }
}
```

Register in `DependencyInjection.cs`:
```csharp
services.AddScoped<IEmailService, SendGridEmailService>();
```
