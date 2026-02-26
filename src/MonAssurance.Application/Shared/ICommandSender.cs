namespace MonAssurance.Application.Shared;

/// <summary>
/// Sends commands to their registered handlers.
/// </summary>
public interface ICommandSender
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default);
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default);
}
