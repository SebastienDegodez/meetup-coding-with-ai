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
