# Clean Architecture .NET Template Design

**Date:** 2026-02-26  
**Project:** MonAssurance v3  
**Goal:** Create a .NET 10 Web API template with Clean Architecture, CQRS without MediatR, and ArchUnit validation

---

## Architecture Overview

**Type:** Web API (REST)  
**Framework:** .NET 10 (Preview/Latest)  
**Persistence:** EF Core + Dapper  
**Testing:** Unit + Integration tests  

**Core Layers:**
```
API (ASP.NET Core Minimal API)
  ↓ references Infrastructure + Domain only
Infrastructure (DI, Repositories, EF Core + Dapper)
  ↓ references Application + Domain
  ↓ discovers handlers by convention
Application (CQRS Handlers, Use Cases)
  ↓ references Domain only
Domain (Aggregates, Value Objects, Entities)
  ↓ zero external dependencies
```

**Key Principles:**
- **Dependency Inversion**: API never references Application directly. Handlers injected via `ICommandHandler<>` / `IQueryHandler<>` interfaces resolved by Infrastructure's convention-based DI
- **Architecture Validation**: ArchUnit tests run on every build to catch layer violations
- **Convention over Configuration**: Handlers auto-registered by naming (`*CommandHandler`, `*QueryHandler`)
- **Test-First Ready**: Project structure supports sociable testing (mock Infrastructure, use real Domain)

---

## Project Structure

```
MonAssurance.sln
src/
  MonAssurance.Domain/
    _Markers/
      IDomainMarker.cs                    # Assembly marker
    README.md                             # Guide for adding entities
  
  MonAssurance.Application/
    _Markers/
      IApplicationMarker.cs               # Assembly marker
    _Contracts/
      ICommandHandler.cs                  # Handler interfaces
      IQueryHandler.cs
    README.md                             # Guide for adding features
  
  MonAssurance.Infrastructure/
    DependencyInjection.cs                # Convention-based registration
    Persistence/
      README.md                           # Guide for repositories
    Services/
      README.md                           # Guide for external services
  
  MonAssurance.Api/
    Program.cs                            # Setup + Swagger
    appsettings.json
    README.md                             # Guide for adding endpoints

tests/
  MonAssurance.UnitTests/
    Application/
      README.md                           # Sociable testing guide
    Domain/
      README.md                           # Domain testing guide
  
  MonAssurance.IntegrationTests/
    Api/
      README.md                           # WebApplicationFactory guide
  
  MonAssurance.ArchitectureTests/
    CleanArchitectureTests.cs             # Test entry point
    ArchUnit/
      DomainLayerRules.cs                 # Domain isolation rules
      ApplicationLayerRules.cs            # Application dependency rules
      ApiLayerRules.cs                    # API layer rules
      InfrastructureLayerRules.cs         # Infrastructure rules
      NamingConventionRules.cs            # Handler naming rules
```

**Structure Notes:**
- Empty template (no sample domain implementation)
- README.md files guide developers on adding features
- ArchUnit rules as separate reusable classes
- Marker interfaces in each layer for assembly discovery

---

## NuGet Packages

**Domain Layer:**
- Zero external dependencies

**Application Layer:**
- References Domain project only
- No external packages

**Infrastructure Layer:**
```bash
dotnet add src/MonAssurance.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/MonAssurance.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add src/MonAssurance.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/MonAssurance.Infrastructure package Dapper
```

**API Layer:**
```bash
dotnet add src/MonAssurance.Api package Swashbuckle.AspNetCore
```

**Test Projects:**
```bash
# UnitTests
dotnet add tests/MonAssurance.UnitTests package xUnit
dotnet add tests/MonAssurance.UnitTests package xUnit.runner.visualstudio
dotnet add tests/MonAssurance.UnitTests package FakeItEasy
dotnet add tests/MonAssurance.UnitTests package Microsoft.NET.Test.Sdk

# IntegrationTests
dotnet add tests/MonAssurance.IntegrationTests package xUnit
dotnet add tests/MonAssurance.IntegrationTests package xUnit.runner.visualstudio
dotnet add tests/MonAssurance.IntegrationTests package Microsoft.AspNetCore.Mvc.Testing
dotnet add tests/MonAssurance.IntegrationTests package Testcontainers.PostgreSql
dotnet add tests/MonAssurance.IntegrationTests package Microsoft.NET.Test.Sdk

# ArchitectureTests
dotnet add tests/MonAssurance.ArchitectureTests package xUnit
dotnet add tests/MonAssurance.ArchitectureTests package xUnit.runner.visualstudio
dotnet add tests/MonAssurance.ArchitectureTests package ArchUnitNET.xUnit
dotnet add tests/MonAssurance.ArchitectureTests package Microsoft.NET.Test.Sdk
```

---

## CQRS Implementation (Without MediatR)

### Handler Contracts

Located in `Application/_Contracts/`:

```csharp
// ICommandHandler.cs - Commands without return value
public interface ICommandHandler<in TCommand>
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

// ICommandHandler.cs - Commands with return value
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

// IQueryHandler.cs - Read operations
public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
```

### Convention-Based Handler Discovery

Located in `Infrastructure/DependencyInjection.cs`:

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
    {
        var applicationAssembly = typeof(IApplicationMarker).Assembly;
        
        // Discover all *CommandHandler classes
        var commandHandlers = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("CommandHandler") && !t.IsInterface && !t.IsAbstract);
            
        foreach (var handler in commandHandlers)
        {
            var interfaces = handler.GetInterfaces()
                .Where(i => i.IsGenericType && 
                       (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));
                        
            foreach (var @interface in interfaces)
                services.AddScoped(@interface, handler);
        }
        
        // Discover all *QueryHandler classes
        var queryHandlers = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("QueryHandler") && !t.IsInterface && !t.IsAbstract);
            
        foreach (var handler in queryHandlers)
        {
            var interfaces = handler.GetInterfaces()
                .Where(i => i.IsGenericType && 
                       i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
                        
            foreach (var @interface in interfaces)
                services.AddScoped(@interface, handler);
        }
        
        return services;
    }
}
```

### Naming Conventions (Enforced by ArchUnit)

- **Commands**: `*Command.cs` → Handlers: `*CommandHandler.cs`
- **Queries**: `*Query.cs` → Handlers: `*QueryHandler.cs`
- **ViewModels**: `*ViewModel.cs` (NOT `*Dto`)

### API Injection Pattern

```csharp
// API never references Application assembly
// Injects handlers via interface, resolved by Infrastructure
app.MapPost("/api/resource", async (
    MyCommand command,
    ICommandHandler<MyCommand, MyResult> handler,
    CancellationToken ct) =>
{
    var result = await handler.HandleAsync(command, ct);
    return Results.Ok(result);
});
```

---

## Architecture Validation with ArchUnit

### Test Entry Point

Located in `ArchitectureTests/CleanArchitectureTests.cs`:

```csharp
public sealed class CleanArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
        => DomainLayerRules.ShouldNotDependOnOtherLayers();
    
    [Fact]
    public void Application_ShouldOnlyDependOnDomain()
        => ApplicationLayerRules.ShouldOnlyDependOnDomain();
    
    [Fact]
    public void Api_ShouldNotReferenceApplication()
        => ApiLayerRules.ShouldNotReferenceApplication();
    
    [Fact]
    public void Infrastructure_CanReferenceDomainAndApplication()
        => InfrastructureLayerRules.CanReferenceDomainAndApplication();
    
    [Fact]
    public void CommandHandlers_ShouldEndWithCommandHandler()
        => NamingConventionRules.CommandHandlersShouldEndWithCommandHandler();
    
    [Fact]
    public void QueryHandlers_ShouldEndWithQueryHandler()
        => NamingConventionRules.QueryHandlersShouldEndWithQueryHandler();
    
    [Fact]
    public void ViewModels_ShouldEndWithViewModel()
        => NamingConventionRules.ViewModelsShouldEndWithViewModel();
}
```

### Architecture Rules

Separate classes in `ArchUnit/` folder:
- **DomainLayerRules.cs**: Validates Domain has zero external dependencies
- **ApplicationLayerRules.cs**: Validates Application only references Domain
- **ApiLayerRules.cs**: Validates API doesn't reference Application
- **InfrastructureLayerRules.cs**: Validates Infrastructure references
- **NamingConventionRules.cs**: Enforces handler/ViewModel naming

---

## Testing Strategy

### Unit Tests
- **Location**: `tests/MonAssurance.UnitTests/Application/`
- **Approach**: Sociable testing - use real Domain objects, mock only Infrastructure
- **Framework**: xUnit + FakeItEasy
- **Speed**: Fast (<100ms per test, no database)

### Integration Tests
- **Location**: `tests/MonAssurance.IntegrationTests/Api/`
- **Approach**: Full API tests with TestContainers
- **Framework**: xUnit + WebApplicationFactory + TestContainers
- **Scope**: End-to-end API validation with real database

### Architecture Tests
- **Location**: `tests/MonAssurance.ArchitectureTests/`
- **Approach**: Compile-time validation of layer dependencies
- **Framework**: ArchUnitNET
- **Execution**: Run on every build

### Test Commands

```bash
# Run all tests
dotnet test

# Run only architecture tests
dotnet test --filter "FullyQualifiedName~ArchitectureTests"

# Watch mode during development
dotnet watch test
```

---

## Key Features

✅ **Empty Structure**: No sample code, just folders and README guides  
✅ **ArchUnit from Day 1**: Architecture violations caught immediately  
✅ **No MediatR**: Convention-based handler discovery  
✅ **API Independence**: API never references Application assembly  
✅ **Clean Dependencies**: Each layer respects dependency rules  
✅ **Test-First Ready**: Structure supports sociable testing  
✅ **EF Core + Dapper**: Hybrid persistence (EF for writes, Dapper for complex reads)  
✅ **Minimal API**: Modern endpoint routing with handler injection  

---

## Next Steps

Implementation plan to be created following this design.
