# MonAssurance.Application

Application layer orchestrates use cases using CQRS pattern (Commands & Queries).

## What Goes Here

- **Commands**: Write operations (create, update, delete)
- **Queries**: Read operations (get, list, search)
- **Handlers**: Execute commands/queries (`*CommandHandler`, `*QueryHandler`)
- **ViewModels**: DTOs for query results (NOT Domain entities)

## Naming Conventions

**CRITICAL**: Follow these conventions for auto-discovery:

- Commands: `*Command.cs` → Handlers: `*CommandHandler.cs`
- Queries: `*Query.cs` → Handlers: `*QueryHandler.cs`
- ViewModels: `*ViewModel.cs` (NOT `*Dto`)

## Example Structure

```
MonAssurance.Application/
  Orders/
    Commands/
      PlaceOrder/
        PlaceOrderCommand.cs
        PlaceOrderCommandHandler.cs
    Queries/
      GetOrder/
        GetOrderQuery.cs
        GetOrderQueryHandler.cs
        OrderViewModel.cs
```

## Handler Registration

Handlers are auto-registered via convention in `Infrastructure/DependencyInjection.cs`.

No manual registration needed - just follow naming conventions!

## Testing

Test handlers with **sociable testing**:
- Use real Domain objects
- Mock only Infrastructure dependencies

See `tests/MonAssurance.UnitTests/Application/README.md`
