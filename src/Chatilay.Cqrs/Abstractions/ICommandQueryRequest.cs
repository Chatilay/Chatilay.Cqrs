namespace Chatilay.Cqrs;

public interface ICommandQueryRequest<out TResponse>;

public interface ICommandQueryRequest : ICommandQueryRequest<Unit>;

public interface IEvent;

public readonly record struct Unit
{
    public static readonly Unit Value = default;

    public static Task<Unit> Task { get; } = System.Threading.Tasks.Task.FromResult(Value);

    public override string ToString() => "()";
}
