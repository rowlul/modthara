using System.Xml;

namespace Modthara.Lari;

internal static class XmlExtensions
{
    public static void ValidateXml(this Stream stream)
    {
        using var reader = XmlReader.Create(stream);

        try
        {
            while (reader.Read())
            {
            }
        }
        catch (XmlException e)
        {
            var info = (IXmlLineInfo)reader;
            throw new LsxMarkupException("Stream has invalid XML data.", (info.LineNumber, info.LinePosition), e);
        }
        finally
        {
            stream.Position = 0;
        }
    }
}
