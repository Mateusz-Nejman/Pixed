using Pixed.Common.Menu;
using Pixed.Common.Selection;
using Pixed.Core;

namespace Pixed.Application.Menu;
internal class CopyPasteMenuRegister(IMenuItemRegistry menuItemRegistry, SelectionManager selectionManager)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly SelectionManager _selectionManager = selectionManager;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Copy", new AsyncCommand(_selectionManager.Copy), null, new("avares://Pixed.Application/Resources/Icons/copy-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Cut", new AsyncCommand(_selectionManager.Cut), null, new("avares://Pixed.Application/Resources/Icons/scissors-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Paste", new AsyncCommand(_selectionManager.Paste), null, new("avares://Pixed.Application/Resources/Icons/paste-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Select All", _selectionManager.SelectAll, new("avares://Pixed.Application/Resources/Icons/tools/tool-rectangle-select.png"));
    }
}
