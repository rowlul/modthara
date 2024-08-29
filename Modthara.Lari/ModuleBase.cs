using Modthara.Lari.Lsx;
using Modthara.Lari.Lsx.Factories;

namespace Modthara.Lari;

/// <summary>
/// Represents <c>Module</c> node.
/// </summary>
public abstract class ModuleBase
{
    public LariUuid Uuid { get; protected init; }

    protected ModuleBase(LariUuid uuid)
    {
        Uuid = uuid;
    }

    public LsxNode ToNode() => LsxNodeFactory.CreateModule(new LsxModuleAttributes(Uuid));
}
