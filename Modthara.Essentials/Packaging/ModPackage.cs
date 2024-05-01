using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Modthara.Lari;
using Modthara.Lari.Pak;

namespace Modthara.Essentials.Packaging;

public class ModPackage : ModMetadata, INotifyPropertyChanged
{
    private string _path;
    private readonly Package _package;
    private PackagedFile? _scriptExtenderConfig;
    private ModFlags _flags;
    private DateTime _lastModified = DateTime.Now;

    public required string Path
    {
        get => _path;
        [MemberNotNull(nameof(_path))]
        set
        {
            _path = value;
            OnPropertyChanged(nameof(Path));
        }
    }

    public required Package Package
    {
        get => _package;
        [MemberNotNull(nameof(_package))]
        init
        {
            _package = value;
            OnPropertyChanged(nameof(Package));
        }
    }

    public PackagedFile? ScriptExtenderConfig
    {
        get => _scriptExtenderConfig;
        set
        {
            _scriptExtenderConfig = value;
            OnPropertyChanged(nameof(ScriptExtenderConfig));
        }
    }

    public required ModFlags Flags
    {
        get => _flags;
        set
        {
            _flags = value;
            OnPropertyChanged(nameof(Flags));
        }
    }

    public DateTime LastModified
    {
        get => _lastModified;
        set
        {
            _lastModified = value;
            OnPropertyChanged(nameof(LastModified));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
