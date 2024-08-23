using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace Modthara.UI.Behaviors;

public class DataGridDragHandler : IDragHandler
{
    public static DataGrid? CurrentRowSource { get; set; }

    /// <inheritdoc />
    public void BeforeDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        var dataGrid = (sender as DataGridRow)?.GetLogicalAncestors().OfType<DataGrid>().FirstOrDefault();
        if (dataGrid != null)
        {
            CurrentRowSource ??= dataGrid;
        }
    }

    /// <inheritdoc />
    public void AfterDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        // DataGrid already set
    }
}
