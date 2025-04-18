using Avalonia.Controls;
using Pixed.Common.Tools;
using Pixed.Core;

namespace Pixed.Application.Menu;

public class SelectionMenu(SelectionManager selectionManager)
{
    private readonly SelectionManager _selectionManager = selectionManager;
    public MenuFlyout GetMenu()
    {
        return new MenuFlyout()
        {
            Items =
            {
                new Avalonia.Controls.MenuItem(){ Header = "Select All", Command = new ActionCommand(SelectAllAction)},
                new Avalonia.Controls.MenuItem(){ Header = "Deselect", Command = new ActionCommand(DeselectAction) }
            }
        };
    }

    private void DeselectAction()
    {
        _selectionManager.Clear();
    }

    private void SelectAllAction()
    {
        _selectionManager.SelectAll();
    }
}