# Clean Architecture .NET Template Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Initialize MonAssurance v3 with Clean Architecture, CQRS without MediatR, and ArchUnit validation

**Architecture:** 4-layer Clean Architecture (Domain → Application → Infrastructure → API) with convention-based handler discovery

**Tech Stack:** .NET 10, ASP.NET Core Minimal API, EF Core + Dapper, xUnit, ArchUnitNET, PostgreSQL

---

## Task 1: Execute Initialization Script

**Files:**
- Execute: `.github/skills/clean-architecture-dotnet/scripts/init-project.sh`
- Creates: Full solution structure with all projects

**Step 1: Make script executable**

```bash
chmod +x ~/.copilot/installed-plugins/copilot-instructions-sebastien-degodez/csharp-clean-architecture-plugin/skills/clean-architecture-dotnet/scripts/init-project.sh
```

**Step 2: Execute initialization script**

Run:
```bash
~/.copilot/installed-plugins/copilot-instructions-sebastien-degodez/csharp-clean-architecture-plugin/skills/clean-architecture-dotnet/scripts/init-project.sh "MonAssurance"
```

Expected: Creates solution, projects, folder structure, marker interfaces, CQRS contracts, ArchUnit tests, builds successfully

**Step 3: Verify solution structure**

Run: `ls -la src/ tests/`

Expected:
```
src/
  MonAssurance.Domain/
  MonAssurance.Application/
  MonAssurance.Infrastructure/
  MonAssurance.Api/

tests/
  MonAssurance.UnitTests/
  MonAssurance.IntegrationTests/
  MonAssurance.ArchitectureTests/
```

**Step 4: Commit initial structure**

```bash
git add .
git commit -m "chore: initialize clean architecture project structure"
```

---

## Task 2: Remove FluentAssertions Package

**Files:**
- Modify: `tests/MonAssurance.UnitTests/MonAssurance.UnitTests.csproj`
- Modify: `tests/MonAssurance.IntegrationTests/MonAssurance.IntegrationTests.csproj`

**Step 1: Remove FluentAssertions from UnitTests**

Run:
```bash
dotnet remove tests/MonAssurance.UnitTests/MonAssurance.UnitTests.csproj package FluentAssertions
```

Expected: Package removed from UnitTests project

**Step 2: Remove FluentAssertions from IntegrationTests**

Run:
```bash
dotnet remove tests/MonAssurance.IntegrationTests/MonAssurance.IntegrationTests.csproj package FluentAssertions
```

Expected: Package removed from IntegrationTests project

**Step 3: Verify build still works**

Run: `dotnet build`

Expected: Build succeeds

**Step 4: Commit package cleanup**

```bash
git add tests/
git commit -m "chore: remove FluentAssertions dependency"
```

---

## Task 3: Add Infrastructure DependencyInjection

**Files:**
- Create: `src/MonAssurance.Infrastructure/DependencyInjection.cs`

**Step 1: Create DependencyInjection.cs**

Create file with convention-based handler registration:

```csharp
using Microsoft.Extensions.DependencyInjection;
using MonAssurance.Application._Contracts;
using MonAssurance.Application;

namespace MonAssurance.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers all application layer services using convention-based discovery.
    /// Handlers are discovered by naming: *CommandHandler, *QueryHandler.
    /// </summary>
    public static IServiceCollection AddApplicationHandlers(
        this IServiceCollection services)
    {
        var applicationAssembly = typeof(IApplicationMarker).Assembly;
        
        // Discover all *CommandHandler classes
        var commandHandlers = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("CommandHandler") && 
                       !t.IsInterface && 
                       !t.IsAbstract);
            
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
            .Where(t => t.Name.EndsWith("QueryHandler") && 
                       !t.IsInterface && 
                       !t.IsAbstract);
            
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

    /// <summary>
    /// Registers Infrastructure services (repositories, external services, DbContext).
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // Register handlers
        services.AddApplicationHandlers();
        
        // TODO: Add EF Core DbContext
        // services.AddDbContext<AppDbContext>(options => ...);
        
        // TODO: Add repositories
        // services.AddScoped<IOrderRepository, OrderRepository>();
        
        return services;
    }
}
```

**Step 2: Verify build**

Run: `dotnet build`

Expected: Build succeeds

**Step 3: Commit DependencyInjection**

```bash
git add src/MonAssurance.Infrastructure/DependencyInjection.cs
git commit -m "feat: add convention-based handler registration"
```

---

## Task 4: Configure API Program.cs

**Files:**
- Modify: `src/MonAssurance.Api/Program.cs`

**Step 1: Replace Program.cs with complete setup**

```csharp
using MonAssurance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddInfrastructure();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "MonAssurance API",
        Version = "v3",
        Description = "Clean Architecture CQRS API for MonAssurance"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map endpoints
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();

// Make the implicit Program class public for WebApplicationFactory
public partial class Program { }
```

**Step 2: Verify build**

Run: `dotnet build`

Expected: Build succeeds

**Step 3: Commit Program.cs**

```bash
git add src/MonAssurance.Api/Program.cs
git commit -m "feat: configure API with Swagger and health check"
```

---

## Task 5: Add README Guides

**Files:**
- Create: `src/MonAssurance.Domain/README.md`
- Create: `src/MonAssurance.Application/README.md`
- Create: `src/MonAssurance.Infrastructure/Persistence/README.md`
- Create: `src/MonAssurance.Infrastructure/Services/README.md`
- Create: `src/MonAssurance.Api/README.md`
- Create: `tests/MonAssurance.UnitTests/Application/README.md`
- Create: `tests/MonAssurance.IntegrationTests/Api/README.md`

**Step 1: Create Domain README**

Create `src/MonAssurance.Domain/README.md`:

```markdown
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
```

**Step 2: Create Application README**

Create `src/MonAssurance.Application/README.md`:

```markdown
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
```

**Step 3: Create Infrastructure Persistence README**

Create `src/MonAssurance.Infrastructure/Persistence/README.md`:

```markdown
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
```

**Step 4: Create Infrastructure Services README**

Create `src/MonAssurance.Infrastructure/Services/README.md`:

```markdown
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
```

**Step 5: Create API README**

Create `src/MonAssurance.Api/README.md`:

```markdown
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
            .WithName("PlaceOrder")
            .WithOpenApi();
            
        // Query endpoint
        group.MapGet("/{orderId:guid}", GetOrder)
            .WithName("GetOrder")
            .WithOpenApi();
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
```

**Step 6: Create UnitTests Application README**

Create `tests/MonAssurance.UnitTests/Application/README.md`:

```markdown
# Application Layer Unit Tests

Test handlers using **sociable testing** strategy.

## Testing Philosophy

- **Use real Domain objects**: Create aggregates, invoke Domain methods
- **Mock only Infrastructure**: Repositories, external services
- **Fast execution**: No database, no network (<100ms per test)

## Pattern

```csharp
public class PlaceOrderCommandHandlerTests
{
    [Fact]
    public async Task WhenPlacingValidOrder_ShouldCreateConfirmedOrder()
    {
        // Arrange - Mock only Infrastructure
        var orderRepository = A.Fake<IOrderRepository>();
        var inventoryService = A.Fake<IInventoryService>();
        var handler = new PlaceOrderCommandHandler(orderRepository, inventoryService);

        var command = new PlaceOrderCommand(/*...*/);

        // Act - Handler uses REAL Domain objects
        var orderId = await handler.HandleAsync(command);

        // Assert - Verify Infrastructure calls AND Domain state
        A.CallTo(() => orderRepository.AddAsync(
            A<Order>.That.Matches(o => 
                o.Id == command.OrderId &&
                o.Status == OrderStatus.Confirmed
            ),
            A<CancellationToken>._
        )).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task WhenPlacingOrderWithInvalidData_ShouldThrowDomainException()
    {
        // Test Domain validation triggers
        var orderRepository = A.Fake<IOrderRepository>();
        var inventoryService = A.Fake<IInventoryService>();
        var handler = new PlaceOrderCommandHandler(orderRepository, inventoryService);

        var invalidCommand = new PlaceOrderCommand(/*invalid data*/);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            handler.HandleAsync(invalidCommand));
        
        // Verify Infrastructure not called
        A.CallTo(() => orderRepository.AddAsync(A<Order>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
```

## Tools

- **xUnit**: Test framework
- **FakeItEasy**: Mocking Infrastructure dependencies
- **Assert**: Built-in xUnit assertions (no FluentAssertions)

## Run Tests

```bash
dotnet test tests/MonAssurance.UnitTests
```

See **@superpowers/skills/application-layer-testing** for complete guide.
```

**Step 7: Create IntegrationTests API README**

Create `tests/MonAssurance.IntegrationTests/Api/README.md`:

```markdown
# API Integration Tests

End-to-end tests for HTTP endpoints using WebApplicationFactory.

## What to Test

- HTTP endpoints return correct status codes
- Request/response serialization works
- Full request pipeline (middleware, validation, handlers)
- Database integration with TestContainers

## Pattern

```csharp
public class OrdersEndpointTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private PostgreSqlContainer _postgres;

    public async Task InitializeAsync()
    {
        // Start PostgreSQL container
        _postgres = new PostgreSqlBuilder().Build();
        await _postgres.StartAsync();

        // Create test API
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace DbContext with test database
                    services.RemoveAll<DbContextOptions<AppDbContext>>();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_postgres.GetConnectionString()));
                });
            });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PlaceOrder_ShouldReturn201Created()
    {
        // Arrange
        var command = new PlaceOrderCommand(/*...*/);

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var orderId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, orderId);
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
```

## Tools

- **WebApplicationFactory**: In-memory API hosting
- **TestContainers**: Real PostgreSQL database
- **xUnit**: Test framework

## Run Tests

```bash
dotnet test tests/MonAssurance.IntegrationTests
```

**Note**: Requires Docker running for TestContainers.
```

**Step 8: Commit README guides**

```bash
git add src/ tests/
git commit -m "docs: add README guides for all layers"
```

---

## Task 6: Run Architecture Tests

**Files:**
- Execute: Architecture tests in `tests/MonAssurance.ArchitectureTests/`

**Step 1: Run all architecture tests**

Run:
```bash
dotnet test --filter "FullyQualifiedName~ArchitectureTests"
```

Expected: All tests pass (Domain isolation, Application dependencies, API layer rules, naming conventions)

**Step 2: Verify specific rules**

Run each rule individually to verify:
```bash
dotnet test --filter "Domain_ShouldNotDependOnOtherLayers"
dotnet test --filter "Application_ShouldOnlyDependOnDomain"
dotnet test --filter "Api_ShouldNotReferenceApplication"
```

Expected: All pass

**Step 3: Run full test suite**

Run:
```bash
dotnet test
```

Expected: All tests pass (including generated UnitTests/IntegrationTests)

**Step 4: Commit verification**

```bash
git add .
git commit -m "test: verify architecture rules pass"
```

---

## Task 7: Add .gitignore and Final Setup

**Files:**
- Create: `.editorconfig` (optional code style)
- Verify: `.gitignore` covers build output

**Step 1: Verify .gitignore**

Check that `.gitignore` includes:
```
bin/
obj/
*.user
*.suo
.vs/
```

**Step 2: Create .editorconfig (optional)**

Create `.editorconfig` with C# conventions:
```ini
root = true

[*.cs]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

# Naming conventions
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.severity = warning
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.symbols = interface
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.style = begins_with_i

dotnet_naming_symbols.interface.applicable_kinds = interface

dotnet_naming_style.begins_with_i.required_prefix = I
dotnet_naming_style.begins_with_i.capitalization = pascal_case
```

**Step 3: Final build verification**

Run:
```bash
dotnet clean
dotnet build
dotnet test
```

Expected: Clean build, all tests pass

**Step 4: Final commit**

```bash
git add .
git commit -m "chore: finalize clean architecture template setup"
```

---

## Completion Checklist

✅ Solution structure created with 4 layers  
✅ Marker interfaces for assembly discovery  
✅ CQRS handler contracts defined  
✅ Convention-based DI registration implemented  
✅ ArchUnit rules validate architecture  
✅ API configured with Swagger and health check  
✅ README guides added for all layers  
✅ FluentAssertions removed  
✅ All tests pass  
✅ Git history clean with semantic commits  

---

## Next Steps After Template

1. **Add first domain entity**: Create aggregate in `Domain/`
2. **Create first command**: Add `*Command.cs` and `*CommandHandler.cs` in `Application/`
3. **Implement repository**: Add interface to `Domain/`, implementation to `Infrastructure/Persistence/`
4. **Add API endpoint**: Create minimal API in `Api/` injecting `ICommandHandler<>`
5. **Write tests**: Unit test handler (sociable), integration test endpoint

**Reference skills:**
- `@superpowers/clean-architecture-dotnet`
- `@superpowers/application-layer-testing`
- `@superpowers/test-driven-development`

---

## Verification Commands

```bash
# Architecture compliance
dotnet test --filter "FullyQualifiedName~ArchitectureTests"

# All tests
dotnet test

# Build
dotnet build

# Run API
dotnet run --project src/MonAssurance.Api

# Check Swagger
open https://localhost:5001/swagger
```
