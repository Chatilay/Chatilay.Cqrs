using System.Reflection;
using Chatilay.Cqrs;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ChatilayCqrsServiceCollectionExtensions
{
    private static readonly Type[] HandlerDefinitions =
    [
        typeof(ICommandQueryHandler<,>),
        typeof(ICommandQueryHandler<>),
        typeof(IEventHandler<>)
    ];

    public static IServiceCollection AddChatilayCqrs(this IServiceCollection services, params Assembly[] assemblies)
        => services.AddChatilayCqrs(options => options.RegisterServicesFromAssemblies(assemblies));

    public static IServiceCollection AddChatilayCqrs(this IServiceCollection services, Action<ChatilayCqrsOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new ChatilayCqrsOptions();
        configure(options);

        if (options.Assemblies.Count == 0)
        {
            throw new InvalidOperationException(
                "Chatilay.Cqrs requires at least one assembly. Use RegisterServicesFromAssembly or RegisterServicesFromAssemblyContaining.");
        }

        services.TryAdd(new ServiceDescriptor(typeof(ISender), typeof(Sender), options.SenderLifetime));

        foreach (var assembly in options.Assemblies)
        {
            foreach (var implementationType in GetLoadableTypes(assembly))
            {
                if (implementationType is not { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
                {
                    continue;
                }

                foreach (var serviceType in implementationType.GetInterfaces())
                {
                    if (!serviceType.IsGenericType)
                    {
                        continue;
                    }

                    if (Array.IndexOf(HandlerDefinitions, serviceType.GetGenericTypeDefinition()) < 0)
                    {
                        continue;
                    }

                    services.TryAddEnumerable(
                        new ServiceDescriptor(serviceType, implementationType, options.HandlerLifetime));
                }
            }
        }

        return services;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}
