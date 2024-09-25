using Avalonia.Controls;
using System;
using System.Windows.Input;
using static Pixed.Menu.MenuBuilder;

namespace Pixed.Menu
{
    internal class MenuItemRegistry(MenuBuilder menuBuilder)
    {
        private readonly MenuBuilder _menuBuilder = menuBuilder;

        public void Register(BaseMenuItem baseMenu, string text, Action action)
        {
            Register(baseMenu, text, new ActionCommand(action));
        }

        public void Register(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
        {
            Register(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
        }

        public void Register(BaseMenuItem baseMenu, NativeMenuItem menuItem)
        {
            _menuBuilder.AddEntry(baseMenu, menuItem);
        }
    }
}
