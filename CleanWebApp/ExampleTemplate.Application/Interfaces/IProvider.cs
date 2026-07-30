namespace ExampleTemplate.Application.Interfaces;

/// <summary>
/// Defines a provider that supplies instances of type <typeparamref name="T"/>.
/// This interface follows the Provider pattern for deferred or lazy instantiation.
/// </summary>
/// <typeparam name="T">The type of object provided by this provider.</typeparam>
public interface IProvider<out T>
{
    /// <summary>
    /// Gets the value provided by this provider.
    /// </summary>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// Implementations may return a new instance on each call or cache and reuse instances.
    /// Check the specific provider implementation for its behavior.
    /// </remarks>
    T Get();
}
