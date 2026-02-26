# MonAssurance.Domain

Domain layer contains pure business logic with **zero external dependencies**.

## What Goes Here

- **Entities**: Business objects with identity (e.g., `Order`, `Customer`)
- **Value Objects**: Immutable objects without identity (e.g., `Address`, `Money`)
- **Aggregates**: Consistency boundaries for entities
- **Domain Events**: Events that represent business state changes
- **Repository Interfaces**: Contracts for data access (implementations in Infrastructure)
- **Domain Services**: Business logic that doesn't belong to a single entity

## Rules

- ❌ NO Entity Framework Core references
- ❌ NO System.Data references
- ❌ NO Infrastructure dependencies
- ✅ Only .NET BCL and other Domain types

## Example Structure

```
MonAssurance.Domain/
  Orders/
    Order.cs                 # Aggregate root
    OrderLine.cs             # Entity
    OrderStatus.cs           # Enum/Value Object
    OrderId.cs               # Strongly-typed ID
    IOrderRepository.cs      # Repository interface
  Shared/
    DomainException.cs
    ValueObject.cs
```

## Validation

Architecture rules enforce Domain isolation. Run:
```bash
dotnet test --filter "Domain_ShouldNotDependOnOtherLayers"
```
