namespace Modthara.Lari.Lsx;

using Position = (int, int);

public class LsxMarkupException(string? message = null, Position? position = null, Exception? inner = null)
    : Exception(message, inner)
{
    public Position? Position { get; } = position;
}
