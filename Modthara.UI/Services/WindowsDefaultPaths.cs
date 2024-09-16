using Modthara.Manager;

namespace Modthara.UI.Services;

public class WindowsDefaultPaths : IPathProvider
{
    public string LocalRoot => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                               @"\Larian Studios";
}
