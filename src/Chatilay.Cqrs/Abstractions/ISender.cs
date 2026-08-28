namespace Chatilay.Cqrs;

public interface ISender
{
    Task<TResponse> Send<TResponse>(ICommandQueryRequest<TResponse> request, CancellationToken cancellationToken = default);

    Task Send(ICommandQueryRequest request, CancellationToken cancellationToken = default);

    Task Publish(IEvent @event, CancellationToken cancellationToken = default);
}
