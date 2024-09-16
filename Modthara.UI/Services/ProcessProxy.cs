using System.Diagnostics;
using System.IO.Abstractions;

namespace Modthara.UI.Services;

public class ProcessProxy : IProcessProxy
{
    private readonly IFileSystem _fileSystem;

    public ProcessProxy(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public Process? Start(ProcessStartInfo info) => Process.Start(info);

    public Process? ShellExecute(string path) =>
        Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });

    public void OpenFolder(string path)
    {
        var attributes = _fileSystem.File.GetAttributes(path);
        if (attributes.HasFlag(FileAttributes.Directory))
        {
            _ = ShellExecute(path);
        }
        else
        {
            throw new InvalidOperationException($"{path} is not a directory.");
        }
    }
}
