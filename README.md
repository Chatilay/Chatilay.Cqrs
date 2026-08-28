# Chatilay.Cqrs

[![CI](https://github.com/Chatilay/Chatilay.Cqrs/actions/workflows/ci.yml/badge.svg)](https://github.com/Chatilay/Chatilay.Cqrs/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Chatilay.Cqrs.svg)](https://www.nuget.org/packages/Chatilay.Cqrs)

A lightweight CQRS / mediator layer built on a single `ICommandQueryRequest` abstraction. There is no `ICommand` / `IQuery` split — both ended up doing the same job, so the package exposes one request type and one handler type. Handler resolution goes through cached generic wrappers, so dispatch does not pay a reflection cost per call.

## Installation

```bash
dotnet add package Chatilay.Cqrs
```

## Registration

```csharp
builder.Services.AddChatilayCqrs(typeof(Program).Assembly);
```

or, with options:

```csharp
builder.Services.AddChatilayCqrs(options =>
{
    options.RegisterServicesFromAssemblyContaining<Program>();
    options.HandlerLifetime = ServiceLifetime.Scoped;
    options.SenderLifetime = ServiceLifetime.Scoped;
});
```

Handlers are discovered by scanning the given assemblies and registered idempotently, so calling `AddChatilayCqrs` twice does not produce duplicates.

## Requests with a response

```csharp
public sealed record GetUserQuery(int Id) : ICommandQueryRequest<UserDto>;

public sealed class GetUserQueryHandler : ICommandQueryHandler<GetUserQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new UserDto(request.Id));
}

var user = await sender.Send(new GetUserQuery(1), cancellationToken);
```

## Requests without a response

```csharp
public sealed record DeleteUserCommand(int Id) : ICommandQueryRequest;

public sealed class DeleteUserCommandHandler : ICommandQueryHandler<DeleteUserCommand>
{
    public Task HandleAsync(DeleteUserCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}

await sender.Send(new DeleteUserCommand(1), cancellationToken);
```

`ICommandQueryRequest` is shorthand for `ICommandQueryRequest<Unit>`, so void requests travel the same path as any other request.

## Events

```csharp
public sealed record UserCreatedEvent(int Id) : IEvent;

public sealed class AuditHandler : IEventHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
        => Task.CompletedTask;
}

await sender.Publish(new UserCreatedEvent(1), cancellationToken);
```

`Publish` resolves handlers from the event's runtime type and invokes them sequentially. Because the parameter is `IEvent` rather than a generic argument, publishing through a base-typed variable still reaches the right handlers.

## API surface

| Type | Purpose |
| --- | --- |
| `ICommandQueryRequest<TResponse>` | A request that returns a response |
| `ICommandQueryRequest` | Shorthand for `ICommandQueryRequest<Unit>` |
| `ICommandQueryHandler<TRequest, TResponse>` | Handles a request |
| `ICommandQueryHandler<TRequest>` | Void handler shorthand, implemented as `HandleAsync` |
| `IEvent` / `IEventHandler<TEvent>` | An event and its handlers |
| `ISender` | `Send` and `Publish` |
| `ChatilayCqrsOptions` | Assembly registration and service lifetimes |
| `Unit` | The void equivalent |

## Requirements

.NET 8.0 or later. The only dependency is `Microsoft.Extensions.DependencyInjection.Abstractions`.

## Building from source

```bash
dotnet test -c Release
dotnet pack -c Release
```

All build output lands under `artifacts/` at the repository root.

## License

MIT — see [LICENSE](LICENSE).
