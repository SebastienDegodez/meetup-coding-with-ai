using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using MonAssurance.Application.Shared;
using MonAssurance.Infrastructure;
using MonAssurance.IntegrationTests.Fakes;
using Xunit;

namespace MonAssurance.IntegrationTests;

public sealed class CqrsIntegrationTests
{
    [Fact]
    public async Task SendAsync_WithCommandHandler_ShouldCallHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddInfrastructure();
        services.AddHandler<TestCommandHandler>();
        
        var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<ICommandSender>();
        var command = new TestCommand { Value = "test" };
        
        // Act
        await sender.SendAsync(command);
        
        // Assert - Handler should be called (tracked in TestCommandHandler.WasCalled)
        var handler = provider.GetRequiredService<ICommandHandler<TestCommand>>();
        Assert.IsType<TestCommandHandler>(handler);
        Assert.True(((TestCommandHandler)handler).WasCalled);
    }

    [Fact]
    public async Task SendAsync_WithQueryHandler_ShouldCallHandlerAndReturnResult()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddInfrastructure();
        services.AddHandler<TestQueryHandler>();
        
        var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<IQuerySender>();
        var query = new TestQuery { Id = 42 };
        
        // Act
        var result = await sender.SendAsync<TestQuery, string>(query);
        
        // Assert
        Assert.Equal("Query result for 42", result);
        var handler = provider.GetRequiredService<IQueryHandler<TestQuery, string>>();
        Assert.IsType<TestQueryHandler>(handler);
        Assert.True(((TestQueryHandler)handler).WasCalled);
    }

    [Fact]
    public async Task SendAsync_WithMockedHandler_ShouldCallMock()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddInfrastructure();
        
        var mockHandler = A.Fake<ICommandHandler<TestCommand>>();
        services.AddScoped<ICommandHandler<TestCommand>>(_ => mockHandler);
        
        var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<ICommandSender>();
        var command = new TestCommand { Value = "mocked" };
        
        // Act
        await sender.SendAsync(command);
        
        // Assert
        A.CallTo(() => mockHandler.HandleAsync(command, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SendAsync_WithScopedHandlers_ShouldResolveNewInstancePerScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddInfrastructure();
        services.AddHandler<TestCommandHandler>();
        
        var provider = services.BuildServiceProvider();
        
        // Act - Create two scopes and send commands
        ICommandHandler<TestCommand> handler1;
        ICommandHandler<TestCommand> handler2;
        
        using (var scope1 = provider.CreateScope())
        {
            var sender1 = scope1.ServiceProvider.GetRequiredService<ICommandSender>();
            await sender1.SendAsync(new TestCommand { Value = "scope1" });
            handler1 = scope1.ServiceProvider.GetRequiredService<ICommandHandler<TestCommand>>();
        }
        
        using (var scope2 = provider.CreateScope())
        {
            var sender2 = scope2.ServiceProvider.GetRequiredService<ICommandSender>();
            await sender2.SendAsync(new TestCommand { Value = "scope2" });
            handler2 = scope2.ServiceProvider.GetRequiredService<ICommandHandler<TestCommand>>();
        }
        
        // Assert - Different instances per scope
        Assert.NotSame(handler1, handler2);
        Assert.IsType<TestCommandHandler>(handler1);
        Assert.IsType<TestCommandHandler>(handler2);
    }
}
