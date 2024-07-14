namespace Modthara.Manager;

/// <summary>
/// Manages mod orders.
/// </summary>
public interface IModOrderManager
{
    /// <summary>
    /// Loads mod order from the specified file path.
    /// </summary>
    /// <param name="path">
    /// The path to the JSON file containing the mod order.
    /// </param>
    /// <returns>
    /// A deserialized <see cref="ModOrder"/>.
    /// </returns>
    ValueTask<ModOrder?> LoadOrderAsync(string path);

    /// <summary>
    /// Saves the specified order to the given file path.
    /// </summary>
    /// <param name="path">
    /// The path to the JSON file where the order entries will be saved.
    /// </param>
    /// <param name="order">
    /// The mod order to save.
    /// </param>
    Task SaveOrderAsync(string path, ModOrder order);
}
