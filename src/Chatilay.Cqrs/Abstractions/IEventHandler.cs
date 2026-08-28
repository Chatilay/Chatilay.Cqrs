namespace Chatilay.Cqrs;

public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}
