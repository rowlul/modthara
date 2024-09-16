using System.Diagnostics;

namespace Modthara.UI.Services;

public interface IProcessProxy
{
    Process? Start(ProcessStartInfo info);
    Process? ShellExecute(string path);
    void OpenFolder(string path);
}
