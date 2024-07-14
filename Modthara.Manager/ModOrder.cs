using System.Text.Json.Serialization;

namespace Modthara.Manager;

[JsonSerializable(typeof(ModOrder))]
public class ModOrder
{
    [JsonRequired]
    [JsonPropertyName("Order")]
    public required IAsyncEnumerable<ModOrderEntry> Entries { get; set; }
}
