namespace Modthara.Lari.Pak;

/// <summary>
/// Represents the exception that is thrown when an LSPK package or its data is in an invalid state.
/// </summary>
public class LspkException(string? message = null, Exception? inner = null) : Exception(message, inner);
