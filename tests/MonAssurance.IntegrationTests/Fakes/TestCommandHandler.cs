using MonAssurance.Application.Shared;

namespace MonAssurance.IntegrationTests.Fakes;

public sealed class TestCommandHandler : ICommandHandler<TestCommand>
{
    public bool WasCalled { get; private set; }
    
    public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        return Task.CompletedTask;
    }
}
