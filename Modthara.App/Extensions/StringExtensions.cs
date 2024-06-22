namespace Modthara.App.Extensions;

public static class StringExtensions
{
    public static bool IsWhiteSpace(this string s)
    {
        foreach (var c in s)
        {
            if (!char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return true;
    }
}
