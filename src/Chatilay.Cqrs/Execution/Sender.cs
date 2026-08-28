namespace Chatilay.Cqrs;

public sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public Task<TResponse> Send<TResponse>(
        ICommandQueryRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wrapper = RequestHandlerWrapperCache<TResponse>.Wrappers.GetOrAdd(
            request.GetType(),
            static requestType =>
            {
                var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, typeof(TResponse));

                return (RequestHandlerWrapperBase<TResponse>)(Activator.CreateInstance(wrapperType)
                    ?? throw new InvalidOperationException($"Could not create a handler wrapper for '{requestType}'."));
            });

        return wrapper.Handle(request, _serviceProvider, cancellationToken);
    }

    public Task Send(ICommandQueryRequest request, CancellationToken cancellationToken = default)
        => Send<Unit>(request, cancellationToken);

    public Task Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var wrapper = EventHandlerWrapperCache.Wrappers.GetOrAdd(
            @event.GetType(),
            static eventType =>
            {
                var wrapperType = typeof(EventHandlerWrapper<>).MakeGenericType(eventType);

                return (EventHandlerWrapperBase)(Activator.CreateInstance(wrapperType)
                    ?? throw new InvalidOperationException($"Could not create an event wrapper for '{eventType}'."));
            });

        return wrapper.Handle(@event, _serviceProvider, cancellationToken);
    }
}
