namespace MonAssurance.Application.Shared;

/// <summary>
/// Sends queries to their registered handlers.
/// </summary>
public interface IQuerySender
{
    Task<TResult> SendAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default);
}
