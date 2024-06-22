using Avalonia.Controls;

using Modthara.App.ViewModels;
using Modthara.Lari;

namespace Modthara.App.Services;

public interface IOrderGridService
{
    IEnumerable<ModPackageViewModel> ViewModels { get; }

    void CreateViewModels(out IEnumerable<ModMetadata> missingMods);

    FlatTreeDataGridSource<ModPackageViewModel> CreateSource();

    IEnumerable<ModPackageViewModel>? FilterMods(string? query);

    void ToggleMod(ModPackageViewModel source, bool newValue);

    void ToggleMods(FlatTreeDataGridSource<ModPackageViewModel> source, bool newValue);
}
