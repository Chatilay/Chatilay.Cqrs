using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Chatilay.Cqrs;

internal abstract class EventHandlerWrapperBase
{
    public abstract Task Handle(IEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

internal sealed class EventHandlerWrapper<TEvent> : EventHandlerWrapperBase
    where TEvent : IEvent
{
    public override async Task Handle(IEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        foreach (var handler in serviceProvider.GetServices<IEventHandler<TEvent>>())
        {
            await handler.Handle((TEvent)@event, cancellationToken).ConfigureAwait(false);
        }
    }
}

internal static class EventHandlerWrapperCache
{
    public static readonly ConcurrentDictionary<Type, EventHandlerWrapperBase> Wrappers = new();
}
