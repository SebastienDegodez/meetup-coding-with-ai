# MonAssurance.Api

API layer exposes HTTP endpoints using ASP.NET Core Minimal APIs.

## What Goes Here

- **Endpoints**: Minimal API route definitions
- **Program.cs**: Application setup and middleware pipeline

## Key Rules

- ❌ **NEVER reference Application assembly**
- ✅ Inject handlers via `ICommandHandler<>` / `IQueryHandler<>`
- ✅ Infrastructure resolves handlers via convention-based DI

## Endpoint Pattern

```csharp
public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");
        
        // Command endpoint
        group.MapPost("/", PlaceOrder)
            .WithName("PlaceOrder");
            
        // Query endpoint
        group.MapGet("/{orderId:guid}", GetOrder)
            .WithName("GetOrder");
    }
    
    private static async Task<IResult> PlaceOrder(
        PlaceOrderCommand command,
        ICommandHandler<PlaceOrderCommand, OrderId> handler,
        CancellationToken ct)
    {
        var orderId = await handler.HandleAsync(command, ct);
        return Results.Created($"/api/orders/{orderId.Value}", orderId);
    }
    
    private static async Task<IResult> GetOrder(
        Guid orderId,
        IQueryHandler<GetOrderQuery, OrderViewModel> handler,
        CancellationToken ct)
    {
        var query = new GetOrderQuery(new OrderId(orderId));
        var result = await handler.HandleAsync(query, ct);
        return Results.Ok(result);
    }
}
```

Register in `Program.cs`:
```csharp
app.MapOrdersEndpoints();
```

## Validation

Architecture tests ensure API doesn't reference Application:
```bash
dotnet test --filter "Api_ShouldNotReferenceApplication"
```
