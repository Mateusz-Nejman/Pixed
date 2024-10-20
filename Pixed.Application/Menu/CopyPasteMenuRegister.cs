﻿using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Selection;

namespace Pixed.Application.Menu;
internal class CopyPasteMenuRegister(IMenuItemRegistry menuItemRegistry, SelectionManager selectionManager)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly SelectionManager _selectionManager = selectionManager;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Copy", new AsyncCommand(_selectionManager.Copy));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Cut", new AsyncCommand(_selectionManager.Cut));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Paste", new AsyncCommand(_selectionManager.Paste));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Select All", _selectionManager.SelectAll);
    }
}
