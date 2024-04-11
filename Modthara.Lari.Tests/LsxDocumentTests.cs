using Modthara.Lari.Lsx;

namespace Modthara.Lari.Tests;

public class LsxDocumentTests
{
    [Fact]
    public void FromStream_RegionWithRoot_ReturnsLsxDocument()
    {
        var expected = new LsxDocument
        {
            Version = new LariVersion { Major = 4, Minor = 0, Revision = 6, Build = 68 },
            Regions =
            [
                new LsxRegion { Id = "Test", RootNode = new LsxNode { Id = "root", Children = [], Attributes = [] } }
            ]
        };

        var xml = """
                  <?xml version="1.0" encoding="UTF-8"?>
                  <save>
                      <version major="4" minor="0" revision="6" build="68" />
                      <region id="Test">
                          <node id="root">
                          </node>
                      </region>
                  </save>
                  """u8.ToArray();

        using var s = new MemoryStream(xml);
        var actual = LsxDocument.FromStream(s);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToStream_RegionWithRoot_ReturnsStream()
    {
        const string expected =
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <save>
                <version major="4" minor="0" revision="6" build="68" />
                <region id="Test">
                    <node id="root" />
                </region>
            </save>
            """;

        var s = new LsxDocument
        {
            Version = new LariVersion { Major = 4, Minor = 0, Revision = 6, Build = 68 },
            Regions =
            [
                new LsxRegion { Id = "Test", RootNode = new LsxNode { Id = "root" } }
            ]
        }.ToStream();
        using var r = new StreamReader(s);
        var actual = r.ReadToEnd().ReplaceLineEndings("\n");
        actual.Should().Be(expected);
    }
}
