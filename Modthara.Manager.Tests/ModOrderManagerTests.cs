using System.IO.Abstractions;

#pragma warning disable CS1998

namespace Modthara.Manager.Tests;

public class ModOrderManagerTests
{
    private async static IAsyncEnumerable<ModOrderEntry> CreateModOrderEntries()
    {
        yield return new ModOrderEntry("1", "a");
        yield return new ModOrderEntry("2", "b");
        yield return new ModOrderEntry("3", "c");
        yield return new ModOrderEntry("4", "d");
    }

    [Fact]
    public async Task LoadOrderAsync_ReturnsModOrder()
    {
        var fs = new FileSystem();
        var sut = new ModOrderManager(fs);

        var expected = new ModOrder { Entries = CreateModOrderEntries() };
        var actual = await sut.LoadOrderAsync("./TestFiles/test_order.json");
        actual.Should().BeEquivalentTo(expected);
    }
}
