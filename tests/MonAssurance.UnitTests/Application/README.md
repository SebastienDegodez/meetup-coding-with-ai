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
