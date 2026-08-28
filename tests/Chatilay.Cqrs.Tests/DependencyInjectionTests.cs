namespace Chatilay.Cqrs.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddChatilayCqrs_RegistersHandlersAndSender()
    {
        var services = new ServiceCollection().AddChatilayCqrs(typeof(DependencyInjectionTests).Assembly);

        Assert.Contains(services, d => d.ServiceType == typeof(ISender) && d.ImplementationType == typeof(Sender));
        Assert.Contains(services, d => d.ServiceType == typeof(ICommandQueryHandler<GetUserNameQuery, string>));
        Assert.Contains(services, d => d.ServiceType == typeof(ICommandQueryHandler<DeleteUserCommand, Unit>));
        Assert.Equal(2, services.Count(d => d.ServiceType == typeof(IEventHandler<UserCreatedEvent>)));
    }

    [Fact]
    public void AddChatilayCqrs_IsIdempotent()
    {
        var services = new ServiceCollection()
            .AddChatilayCqrs(typeof(DependencyInjectionTests).Assembly)
            .AddChatilayCqrs(typeof(DependencyInjectionTests).Assembly);

        Assert.Equal(2, services.Count(d => d.ServiceType == typeof(IEventHandler<UserCreatedEvent>)));
        Assert.Equal(1, services.Count(d => d.ServiceType == typeof(ISender)));
    }

    [Fact]
    public void AddChatilayCqrs_RespectsCustomLifetime()
    {
        var services = new ServiceCollection().AddChatilayCqrs(options =>
        {
            options.HandlerLifetime = ServiceLifetime.Transient;
            options.SenderLifetime = ServiceLifetime.Transient;
            options.RegisterServicesFromAssemblyContaining<DependencyInjectionTests>();
        });

        Assert.All(
            services.Where(d => d.ServiceType == typeof(ICommandQueryHandler<GetUserNameQuery, string>)),
            d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AddChatilayCqrs_ThrowsWhenNoAssemblyRegistered()
        => Assert.Throws<InvalidOperationException>(() => new ServiceCollection().AddChatilayCqrs(_ => { }));
}
