namespace Chatilay.Cqrs.Tests;

public sealed record GetUserNameQuery(int UserId) : ICommandQueryRequest<string>;

public sealed class GetUserNameQueryHandler : ICommandQueryHandler<GetUserNameQuery, string>
{
    public Task<string> Handle(GetUserNameQuery request, CancellationToken cancellationToken)
        => Task.FromResult($"user-{request.UserId}");
}

public sealed record CreateUserCommand(string Name) : ICommandQueryRequest<int>;

public sealed class CreateUserCommandHandler : ICommandQueryHandler<CreateUserCommand, int>
{
    public Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        => Task.FromResult(request.Name.Length);
}

public sealed record DeleteUserCommand(int UserId) : ICommandQueryRequest;

public sealed class DeleteUserCommandHandler : ICommandQueryHandler<DeleteUserCommand>
{
    public static readonly List<int> Deleted = [];

    public Task HandleAsync(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        Deleted.Add(request.UserId);
        return Task.CompletedTask;
    }
}

public sealed record UnhandledQuery : ICommandQueryRequest<string>;

public sealed record UserCreatedEvent(int UserId) : IEvent;

public sealed class AuditEventHandler : IEventHandler<UserCreatedEvent>
{
    public static readonly List<int> Handled = [];

    public Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
    {
        Handled.Add(@event.UserId);
        return Task.CompletedTask;
    }
}

public sealed class NotificationEventHandler : IEventHandler<UserCreatedEvent>
{
    public static readonly List<int> Handled = [];

    public Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
    {
        Handled.Add(@event.UserId);
        return Task.CompletedTask;
    }
}
