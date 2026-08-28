using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Chatilay.Cqrs;

public sealed class ChatilayCqrsOptions
{
    private readonly HashSet<Assembly> _assemblies = [];

    public IReadOnlyCollection<Assembly> Assemblies => _assemblies;

    public ServiceLifetime HandlerLifetime { get; set; } = ServiceLifetime.Scoped;

    public ServiceLifetime SenderLifetime { get; set; } = ServiceLifetime.Scoped;

    public ChatilayCqrsOptions RegisterServicesFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        _assemblies.Add(assembly);
        return this;
    }

    public ChatilayCqrsOptions RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var assembly in assemblies)
        {
            RegisterServicesFromAssembly(assembly);
        }

        return this;
    }

    public ChatilayCqrsOptions RegisterServicesFromAssemblyContaining<TMarker>()
        => RegisterServicesFromAssembly(typeof(TMarker).Assembly);

    public ChatilayCqrsOptions RegisterServicesFromAssemblyContaining(Type markerType)
    {
        ArgumentNullException.ThrowIfNull(markerType);
        return RegisterServicesFromAssembly(markerType.Assembly);
    }
}
