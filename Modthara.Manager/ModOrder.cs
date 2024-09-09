using System.Text.Json.Serialization;

namespace Modthara.Manager;

[JsonSerializable(typeof(ModOrder))]
public class ModOrder
{
    [JsonRequired]
    [JsonPropertyName("Order")]
    public required List<ModOrderEntry> Entries { get; set; }
}
