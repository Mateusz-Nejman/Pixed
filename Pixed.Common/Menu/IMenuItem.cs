using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Pixed.Common.Menu;
public interface IMenuItem
{
    public string? Header { get; set; }
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public Uri? Icon { get; set; }
    public List<IMenuItem> Items { get; set; }

    public Avalonia.Controls.MenuItem ToAvaloniaMenuItem();
}
