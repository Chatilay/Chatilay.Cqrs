namespace Chatilay.Cqrs.Tests;

public class SenderTests
{
    private static ServiceProvider BuildProvider()
        => new ServiceCollection()
            .AddChatilayCqrs(typeof(SenderTests).Assembly)
            .BuildServiceProvider(validateScopes: true);

    [Fact]
    public async Task Send_ResolvesQueryHandler()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new GetUserNameQuery(7));

        Assert.Equal("user-7", result);
    }

    [Fact]
    public async Task Send_ResolvesCommandHandler()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new CreateUserCommand("chatilay"));

        Assert.Equal(8, result);
    }

    [Fact]
    public async Task Send_ResolvesVoidCommandHandler()
    {
        DeleteUserCommandHandler.Deleted.Clear();

        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Send(new DeleteUserCommand(42));

        Assert.Equal([42], DeleteUserCommandHandler.Deleted);
    }

    [Fact]
    public async Task Send_ThrowsWhenHandlerMissing()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => sender.Send(new UnhandledQuery()));
    }

    [Fact]
    public async Task Send_ThrowsWhenRequestIsNull()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sender.Send<string>(null!));
    }

    [Fact]
    public async Task Publish_InvokesAllHandlersForRuntimeType()
    {
        AuditEventHandler.Handled.Clear();
        NotificationEventHandler.Handled.Clear();

        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        IEvent @event = new UserCreatedEvent(3);
        await sender.Publish(@event);

        Assert.Equal([3], AuditEventHandler.Handled);
        Assert.Equal([3], NotificationEventHandler.Handled);
    }

    [Fact]
    public async Task Publish_DoesNotThrowWhenNoHandlerRegistered()
    {
        await using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Publish(new UnhandledEvent());
    }

    private sealed record UnhandledEvent : IEvent;
}
