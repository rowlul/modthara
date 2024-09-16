using Avalonia.Controls;
using Avalonia.Interactivity;

using Modthara.UI.ViewModels;

namespace Modthara.UI.Views;

public partial class PackagesView : UserControl
{
    public PackagesView()
    {
        InitializeComponent();
    }

    private async void ToggleSearchButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        // wait for visibility property
        await Task.Delay(1);
        ModSearchTextBox.Focus();
    }
}
