using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Windows.Input;

namespace Pixed.Common.Menu;
public interface IMenuItemRegistry
{
    public void Register(BaseMenuItem baseMenu, string text, Action action, Uri? icon = null);

    public void Register(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null, Uri? icon = null);

    public void Register(BaseMenuItem baseMenu, IMenuItem menuItem);
}