namespace Chatilay.Cqrs;

public interface ICommandQueryHandler<in TRequest, TResponse>
    where TRequest : ICommandQueryRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface ICommandQueryHandler<in TRequest> : ICommandQueryHandler<TRequest, Unit>
    where TRequest : ICommandQueryRequest
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);

    async Task<Unit> ICommandQueryHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await HandleAsync(request, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}
