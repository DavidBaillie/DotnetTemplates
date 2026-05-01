namespace SukkotStore.WebApp.Interfaces;

public interface IProvider<out T>
{
    /// <summary>
    /// Get the value provided by this provider.
    /// </summary>
    /// <returns>The <typeparamref name="T" /> provided by this provider.</returns>
    T Get();
}
