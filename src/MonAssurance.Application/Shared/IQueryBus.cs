namespace MonAssurance.Application.Shared;

/// <summary>
/// Bus for dispatching queries to their registered handlers.
/// </summary>
public interface IQueryBus
{
    Task<TResult> SendAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default);
}
