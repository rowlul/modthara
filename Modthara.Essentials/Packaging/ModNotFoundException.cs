using Modthara.Lari;

namespace Modthara.Essentials.Packaging;

public class ModNotFoundException(string? message = null, LariUuid? uuid = null, Exception? inner = null)
    : Exception(message, inner)
{
    public LariUuid? Uuid { get; } = uuid;
}
