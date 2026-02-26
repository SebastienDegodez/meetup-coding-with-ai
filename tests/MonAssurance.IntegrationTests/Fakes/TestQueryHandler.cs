using MonAssurance.Application.Shared;

namespace MonAssurance.IntegrationTests.Fakes;

public sealed class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public bool WasCalled { get; private set; }
    
    public Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        return Task.FromResult($"Query result for {query.Id}");
    }
}
