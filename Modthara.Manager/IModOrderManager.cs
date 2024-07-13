namespace Modthara.Manager;

public interface IModOrderManager
{
    /// <summary>
    /// Loads order entries from the specified file path.
    /// </summary>
    /// <param name="path">
    /// The path to the file containing the order entries.
    /// </param>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{ModOrderEntry}"/> containing the order entries.
    /// </returns>
    IAsyncEnumerable<ModOrderEntry?> LoadOrderAsync(string path);

    /// <summary>
    /// Saves the specified order entries to the given file path.
    /// </summary>
    /// <param name="path">
    /// The path to the file where the order entries will be saved.
    /// </param>
    /// <param name="orderEntries">
    /// The order entries to save.
    /// </param>
    Task SaveOrderAsync(string path, IEnumerable<ModOrderEntry> orderEntries);
}
