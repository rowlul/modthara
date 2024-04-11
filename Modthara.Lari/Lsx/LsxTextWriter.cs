using System.Text;
using System.Xml;

namespace Modthara.Lari.Lsx;

public class LsxTextWriter : XmlTextWriter
{
    public LsxTextWriter(Stream stream) : base(stream, Encoding.UTF8)
    {
        this.Formatting = Formatting.Indented;
        this.IndentChar = ' ';
        this.Indentation = 4;
    }

    public override void WriteStartDocument()
    {
        WriteRaw($"""<?xml version="1.0" encoding="{Encoding.UTF8.WebName.ToUpper()}"?>""");
    }
}
