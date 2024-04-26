using Modthara.Lari;

namespace Modthara.Essentials.Abstractions;

public interface IOrderManager
{
    Task<ModSettings> ReadOrderAsync(string path);
    Task SaveOrderAsync(string path, ModSettings modSettings);
}
