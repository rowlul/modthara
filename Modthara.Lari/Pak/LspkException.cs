namespace Modthara.Lari.Pak;

/// <summary>
/// Represents the exception that is thrown when an LSPK package or its data is in an invalid state.
/// </summary>
public class LspkException : Exception
{
    public int? Version { get; }

    public LspkException(string? message = null, int? version = null, Exception? inner = null) : base(message, inner)
    {
        Version = version;
    }
}
