using System.Text.Json.Serialization;

namespace Modthara.UI.Services;

[JsonSerializable(typeof(ConfigurationModel))]
public class ConfigurationModel
{
    public required string LocalLarianFolder { get; set; }
}
