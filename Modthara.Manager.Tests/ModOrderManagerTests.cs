using System.IO.Abstractions;

namespace Modthara.Manager.Tests;

public class ModOrderManagerTests
{
    private readonly static List<ModOrderEntry> ModOrderEntries =
    [
        new("1", "a"),
        new("2", "b"),
        new("3", "c"),
        new("4", "d")
    ];

    [Fact]
    public async Task LoadOrderAsync_ReturnsModOrder()
    {
        var fs = new FileSystem();
        var sut = new ModOrderManager(fs);

        var expected = new ModOrder { Entries = ModOrderEntries };
        var actual = await sut.LoadOrderAsync("./TestFiles/test_order.json");
        actual.Should().BeEquivalentTo(expected);
    }
}
