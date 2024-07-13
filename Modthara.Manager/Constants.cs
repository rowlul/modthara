using System.Text.Json;

using Modthara.Lari;

namespace Modthara.Manager;

internal static class Constants
{
    public const int StreamBufferSize = 0x1000;

    public static readonly JsonSerializerOptions JsonSerializerOptions =
        new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static readonly IEnumerable<LariUuid> IgnoredModUuids =
    [
        new LariUuid("991c9c7a-fb80-40cb-8f0d-b92d4e80e9b1"),
        new LariUuid("28ac9ce2-2aba-8cda-b3b5-6e922f71b6b8"),
        new LariUuid("ed539163-bb70-431b-96a7-f5b2eda5376b"),
        new LariUuid("3d0c5ff8-c95d-c907-ff3e-34b204f1c630"),
        new LariUuid("e5c9077e-1fca-4f24-b55d-464f512c98a8"),
        new LariUuid("9dff4c3b-fda7-43de-a763-ce1383039999"),
        new LariUuid("e842840a-2449-588c-b0c4-22122cfce31b"),
        new LariUuid("b176a0ac-d79f-ed9d-5a87-5c2c80874e10"),
        new LariUuid("e0a4d990-7b9b-8fa9-d7c6-04017c6cf5b1"),
        new LariUuid("77a2155f-4b35-4f0c-e7ff-4338f91426a4"),
        new LariUuid("ee4989eb-aab8-968f-8674-812ea2f4bfd7"),
        new LariUuid("b77b6210-ac50-4cb1-a3d5-5702fb9c744c"),
    ];
}
