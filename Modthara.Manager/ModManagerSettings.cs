using System.Text.Json.Serialization;

namespace Modthara.Manager;

[JsonSerializable(typeof(ModManagerSettings))]
public class ModManagerSettings
{
    [JsonRequired]
    [JsonPropertyName("ModsPath")]
    public required string _modsPath { get; set; }

    [JsonRequired]
    [JsonPropertyName("ModsettingsPath")]
    public required string _modsettingsPath { get; set; }

    [JsonRequired]
    [JsonPropertyName("BinPath")]
    public required string _binPath { get; set; }

    [JsonRequired]
    [JsonPropertyName("DataPath")]
    public required string _dataPath { get; set; }

    [JsonConstructor]
    public ModManagerSettings() {}
    public ModManagerSettings(string ModsPath, string ModsettingsPath, string BinPath, string DataPath)
    {
        _modsPath = ModsPath;
        _modsettingsPath = ModsettingsPath;
        _binPath = BinPath;
        _dataPath = DataPath;
    }
}
