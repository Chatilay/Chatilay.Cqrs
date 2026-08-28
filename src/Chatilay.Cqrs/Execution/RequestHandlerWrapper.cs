using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Chatilay.Cqrs;

internal abstract class RequestHandlerWrapperBase<TResponse>
{
    public abstract Task<TResponse> Handle(
        ICommandQueryRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

internal sealed class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerWrapperBase<TResponse>
    where TRequest : ICommandQueryRequest<TResponse>
{
    public override Task<TResponse> Handle(
        ICommandQueryRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var handler = serviceProvider.GetService<ICommandQueryHandler<TRequest, TResponse>>()
                      ?? throw new InvalidOperationException(
                          $"No handler registered for '{typeof(TRequest)}'. Expected ICommandQueryHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}>.");

        return handler.Handle((TRequest)request, cancellationToken);
    }
}

internal static class RequestHandlerWrapperCache<TResponse>
{
    public static readonly ConcurrentDictionary<Type, RequestHandlerWrapperBase<TResponse>> Wrappers = new();
}
