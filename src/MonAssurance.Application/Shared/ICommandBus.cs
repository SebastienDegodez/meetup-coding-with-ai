namespace MonAssurance.Application.Shared;

/// <summary>
/// Bus for dispatching commands to their registered handlers.
/// </summary>
public interface ICommandBus
{
    Task PublishAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default);
    Task<TResult> PublishAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default);
}
