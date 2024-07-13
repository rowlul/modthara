﻿using System.Text.Json.Serialization;

using Modthara.Lari;

namespace Modthara.Manager;

[JsonSerializable(typeof(ModOrderEntry))]
public class ModOrderEntry : ModuleBase
{
    [JsonRequired]
    [JsonPropertyName("UUID")]
    [JsonPropertyOrder(0)]
    public new string Uuid { get; init; }

    [JsonRequired]
    [JsonPropertyName("Name")]
    [JsonPropertyOrder(1)]
    public string Name { get; init; }

    public ModOrderEntry(string uuid, string name) : base(new LariUuid(uuid))
    {
        Uuid = uuid;
        Name = name;
    }

    public ModOrderEntry(Module module) : this(module.Uuid.Value, module.Name)
    {
    }
}
