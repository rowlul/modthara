namespace Modthara.Lari.Lsx;

public class LsxMissingElementException(string elementName)
    : Exception($"Element '{elementName}' is missing, null, or empty.");
