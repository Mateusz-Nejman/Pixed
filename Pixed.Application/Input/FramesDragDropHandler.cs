using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Pixed.Application.ViewModels;
using Pixed.Common.Models;

namespace Pixed.Application.Input;

internal class FramesDragDropHandler : DropHandlerBase
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            return Validate<Frame>(listBox, e, sourceContext, targetContext, false);
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is ListBox listBox)
        {
            return Validate<Frame>(listBox, e, sourceContext, targetContext, true);
        }
        return false;
    }

    private static bool Validate<T>(ListBox listBox, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute) where T : Frame
    {
        if (sourceContext is not T sourceItem
            || targetContext is not FramesSectionViewModel vm
            || listBox.GetVisualAt(e.GetPosition(listBox)) is not Control targetControl
            || targetControl.DataContext is not T targetItem)
        {
            return false;
        }

        var items = vm.Frames;
        var sourceIndex = items.IndexOf(sourceItem);
        var targetIndex = items.IndexOf(targetItem);

        if (sourceIndex < 0 || targetIndex < 0)
        {
            return false;
        }

        switch (e.DragEffects)
        {
            case DragDropEffects.Move:
                {
                    if (bExecute)
                    {
                        MoveItem(items, sourceIndex, targetIndex);
                    }
                    return true;
                }
            case DragDropEffects.Link:
                {
                    if (bExecute)
                    {
                        SwapItem(items, sourceIndex, targetIndex);
                    }
                    return true;
                }
            default:
                return false;
        }
    }
}
