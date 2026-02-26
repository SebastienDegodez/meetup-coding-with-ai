# Persistence Layer

Implements data access using EF Core + Dapper hybrid approach.

## What Goes Here

- **DbContext**: EF Core configuration
- **Repositories**: Implementations of Domain repository interfaces
- **Entity Configurations**: EF Core mappings
- **Migrations**: Database schema changes

## Pattern

- **EF Core**: Write operations (Commands)
- **Dapper**: Complex read operations (Queries requiring joins/performance)

## Example Structure

```
Persistence/
  AppDbContext.cs
  Repositories/
    OrderRepository.cs        # Implements IOrderRepository
  Configurations/
    OrderConfiguration.cs     # EF Core mapping
  Migrations/
    20260226_InitialCreate.cs
```

## Repository Implementation

```csharp
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    
    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _context.Orders.AddAsync(order, ct);
        await _context.SaveChangesAsync(ct);
    }
    
    public async Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default)
    {
        return await _context.Orders.FindAsync(new object[] { id }, ct);
    }
}
```

Register in `DependencyInjection.cs`:
```csharp
services.AddScoped<IOrderRepository, OrderRepository>();
```
