using Pixed.Common.Menu;
using Pixed.Common.Tools;
using Pixed.Core;

namespace Pixed.Application.Menu;
internal class CopyPasteMenuRegister(IMenuItemRegistry menuItemRegistry, SelectionManager selectionManager)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly SelectionManager _selectionManager = selectionManager;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Copy", new AsyncCommand(_selectionManager.Copy), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_copy_32_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Cut", new AsyncCommand(_selectionManager.Cut), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_cut_24_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Paste", new AsyncCommand(_selectionManager.Paste), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_clipboard_paste_32_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Select All", _selectionManager.SelectAll, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_copy_select_24_regular.svg"));
    }
}
