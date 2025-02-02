﻿using Pixed.Common.Menu;
using Pixed.Core;
using System;
using System.Windows.Input;

namespace Pixed.Application.Menu;
internal class MenuItemRegistry(MenuBuilder menuBuilder) : IMenuItemRegistry
{
    private readonly MenuBuilder _menuBuilder = menuBuilder;

    public void Register(BaseMenuItem baseMenu, string text, Action action, Uri? icon = null)
    {
        Register(baseMenu, text, new ActionCommand(action), null, icon);
    }

    public void Register(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null, Uri? icon = null)
    {
        Register(baseMenu, new MenuItem(text) { Command = command, CommandParameter = commandParameter, Icon = icon });
    }

    public void Register(BaseMenuItem baseMenu, IMenuItem menuItem)
    {
        _menuBuilder.AddEntry(baseMenu, menuItem);
    }
}
