namespace Modthara.Lari.Lsx;

public class LsxMissingElementException : Exception
{
    public LsxMissingElementException(string elementName) :
        base($"Element '{elementName}' is missing, null, or empty.")
    {
    }
}
