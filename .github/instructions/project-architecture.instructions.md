---
description: "Use when creating, modifying, or reviewing C# files. Covers Clean Architecture layers, CQRS conventions, DI registration, and testing patterns."
applyTo: "**/*.cs"
---

# Project Architecture

## Solution Structure

4-layer Clean Architecture with custom CQRS (no MediatR).

```
src/
  {Project}.Api/              → Minimal API endpoints, Swagger, health checks
  {Project}.Application/      → Use case handlers, CQRS interfaces (Shared/)
  {Project}.Domain/           → Pure business logic, aggregates, value objects
  {Project}.Infrastructure/   → DI registration, CQRS bus implementations
tests/
  {Project}.UnitTests/        → Domain/ and Application/ unit tests
  {Project}.IntegrationTests/ → Architecture validation (NetArchTest), CQRS integration
```

## Layer Dependency Rules (enforced by NetArchTest)

- **Domain** → zero dependencies on other layers or frameworks
- **Application** → depends only on Domain
- **Infrastructure** → depends on Application + Domain; never on Api
- **Api** → depends on Infrastructure; never directly on Application

## Feature Organization

Organize code by feature, then by handler type:

```
Application/
  {Feature}/
    Commands/
      {UseCase}/
        {UseCase}Command.cs
        {UseCase}CommandHandler.cs
    Queries/
      {UseCase}/
        {UseCase}Query.cs
        {UseCase}QueryHandler.cs
```

## Marker Interfaces

Each layer exposes a marker interface for assembly scanning:
- `IDomainMarker` in Domain
- `IApplicationMarker` in Application

Use `typeof(IApplicationMarker).Assembly` for reflection-based discovery.

## CQRS Conventions

### Handler Interfaces (in `Application/Shared/`)

| Interface | Purpose |
|-----------|---------|
| `ICommandHandler<TCommand>` | Void command |
| `ICommandHandler<TCommand, TResult>` | Command with result |
| `IQueryHandler<TQuery, TResult>` | Query with result |

### Bus Interfaces (in `Application/Shared/`)

| Interface | Method |
|-----------|--------|
| `ICommandBus` | `PublishAsync<TCommand>(...)` / `PublishAsync<TCommand, TResult>(...)` |
| `IQueryBus` | `SendAsync<TQuery, TResult>(...)` |

### Handler Naming

- `{UseCase}{Type}Handler` — e.g., `CheckEligibilityQueryHandler`
- Command/Query DTOs: `{UseCase}{Type}` — e.g., `CheckEligibilityQuery`

### Command/Query DTOs

Use `sealed` classes with `init`-only properties:

```csharp
public sealed class MyCommand
{
    public string Value { get; init; } = string.Empty;
}
```

## Dependency Injection

### Extension Methods (in Infrastructure)

- `AddInfrastructure()` — registers buses as scoped
- `AddHandler<THandler>()` — registers a single handler via reflection
- `AddApplicationHandlers()` — scans Application assembly, auto-registers all handlers

### Lifetime

All handlers and buses are registered as **Scoped** (one per HTTP request).

### API Registration

```csharp
builder.Services.AddInfrastructure();
builder.Services.AddApplicationHandlers();
```

## Guard Clauses

Use static throw helpers, not manual `if` checks:

```csharp
ArgumentNullException.ThrowIfNull(command);
```

## Testing Conventions

### Test Project Mapping

| Source Layer | Test Location |
|-------------|---------------|
| Domain | `UnitTests/Domain/` |
| Application | `UnitTests/Application/` |
| Architecture rules | `IntegrationTests/CleanArchitectureTests.cs` |
| CQRS wiring | `IntegrationTests/CqrsIntegrationTests.cs` |

### Test Naming

```
When{Condition}_Should{ExpectedOutcome}()
{Layer}_ShouldNotHaveDependencyOn_{OtherLayer}()
```

### Mocking Rules

- **Mock only infrastructure** (repositories, external services)
- **Use real domain objects** — sociable unit testing
- **FakeItEasy** syntax:

```csharp
var mock = A.Fake<ICommandHandler<MyCommand>>();
A.CallTo(() => mock.HandleAsync(command, A<CancellationToken>._))
    .MustHaveHappenedOnceExactly();
```

### Test Fakes (IntegrationTests/Fakes/)

- Sealed classes with `init` properties
- State-tracking handlers (`WasCalled` pattern) for integration tests
