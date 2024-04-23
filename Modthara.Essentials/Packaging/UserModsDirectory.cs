using Modthara.Essentials.Abstractions;

namespace Modthara.Essentials.Packaging;

public class UserModsDirectory : IModsDirectory
{
    private readonly string _path;

    private readonly IModPackageService _modPackageService;

    private readonly List<ModPackage> _packages = [];

    /// <inheritdoc />
    public IReadOnlyList<ModPackage> Packages => _packages;

    public UserModsDirectory(string path, IModPackageService modPackageService)
    {
        _path = path;
        _modPackageService = modPackageService;
    }

    /// <inheritdoc />
    public async Task LoadPackagesAsync(Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null)
    {
        await foreach (var package in _modPackageService.ReadPackagesAsync(_path, onException, packageReadCallback).ConfigureAwait(false))
        {
            _packages.Add(package);
        }
    }

    /// <inheritdoc />
    public int CountPackages() => _modPackageService.CountPackages(_path);
}
