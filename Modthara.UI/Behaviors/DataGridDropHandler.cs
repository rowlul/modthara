using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace Modthara.UI.Behaviors;

public enum DragDirection
{
    Up,
    Down
}

public class DataGridDropHandler<T> : DropHandlerBase where T : class
{
    public DragDirection? Direction;

    public DataGrid? Source;
    public IList<T> SourceList = null!;
    public int SourceIndex = -1;

    public DataGrid Destination = null!;
    public IList<T> DestinationList = null!;
    public int DestinationIndex = -1;

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext,
        object? targetContext, object? state)
    {
        if (DataGridDragHandler.CurrentRowSource is not { } srcGrid ||
            sender is not DataGrid destGrid ||
            sourceContext is not T srcContext ||
            srcGrid.ItemsSource is not DataGridCollectionView { SourceCollection: IList<T> srcList } ||
            destGrid.ItemsSource is not DataGridCollectionView { SourceCollection: IList<T> destList } ||
            srcList.IndexOf(srcContext) == -1 ||
            destGrid.GetVisualAt(e.GetPosition(destGrid),
                v => v.FindDescendantOfType<DataGridCell>() is not null) is not Control
            {
                DataContext: T destContext
            } visual)
        {
            return false;
        }

        var cell = visual.FindDescendantOfType<DataGridCell>()!;
        Direction = cell.DesiredSize.Height / 2 > e.GetPosition(cell).Y ? DragDirection.Up : DragDirection.Down;

        Source = srcGrid;
        SourceList = srcList;
        SourceIndex = srcList.IndexOf(srcContext);

        Destination = destGrid;
        DestinationList = destList;
        DestinationIndex = destList.IndexOf(destContext);

        return true;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext,
        object? state)
    {
        if (!Validate(sender, e, sourceContext))
        {
            DataGridDragHandler.CurrentRowSource = null;
            return false;
        }

        if (!Equals(Source, Destination) && Direction == DragDirection.Down)
        {
            DestinationIndex++;
        }
        else if (SourceIndex > DestinationIndex && Direction == DragDirection.Down)
        {
            DestinationIndex++;
        }
        else if (SourceIndex < DestinationIndex && Direction == DragDirection.Up)
        {
            DestinationIndex--;
        }

        MoveItem(SourceList, DestinationList, SourceIndex, DestinationIndex);
        Destination.SelectedIndex = DestinationIndex;
        Destination.ScrollIntoView(DestinationList[DestinationIndex], null);

        DataGridDragHandler.CurrentRowSource = null;

        return true;
    }

    public override void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (!Validate(sender, e, sourceContext))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        string className = Direction switch
        {
            DragDirection.Down => DraggingDownClassName,
            DragDirection.Up => DraggingUpClassName,
            _ => throw new InvalidOperationException($"Invalid drag direction: {Direction}")
        };
        Destination.Classes.Add(className);

        e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
        e.Handled = true;
    }

    public override void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (!Validate(sender, e, sourceContext))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
        e.Handled = true;

        (string toAdd, string toRemove) classUpdate = Direction switch
        {
            DragDirection.Down => (DraggingDownClassName, DraggingUpClassName),
            DragDirection.Up => (DraggingUpClassName, DraggingDownClassName),
            _ => throw new InvalidOperationException($"Invalid drag direction: {Direction}")
        };

        if (Destination.Classes.Contains(classUpdate.toAdd))
        {
            return;
        }

        Destination.Classes.Remove(classUpdate.toRemove);
        Destination.Classes.Add(classUpdate.toAdd);
    }

    public override void Leave(object? sender, RoutedEventArgs e)
    {
        base.Leave(sender, e);
        RemoveDraggingClass(sender as DataGrid);
    }

    public override void Drop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        RemoveDraggingClass(sender as DataGrid);
        base.Drop(sender, e, sourceContext, targetContext);
        DataGridDragHandler.CurrentRowSource = null;
    }

    private static void RemoveDraggingClass(DataGrid? dg)
    {
        if (dg is not null && !dg.Classes.Remove(DraggingUpClassName))
        {
            dg.Classes.Remove(DraggingDownClassName);
        }
    }

    private bool Validate(object? sender, DragEventArgs e, object? sourceContext)
    {
        // ReSharper disable once IntroduceOptionalParameters.Local
        return Validate(sender, e, sourceContext, null, null);
    }

    private const string DraggingUpClassName = "dragging-up";
    private const string DraggingDownClassName = "dragging-down";
}
