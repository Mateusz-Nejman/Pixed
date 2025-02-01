using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using Pixed.Common.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Pixed.Application.Menu;
internal class MenuItem(string? header) : IMenuItem
{
    public string? Header { get; set; } = header;
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public Uri? Icon { get; set; }
    public List<IMenuItem>? Items { get; set; }

    public MenuItem() : this(null) { }

    public Avalonia.Controls.MenuItem ToAvaloniaMenuItem()
    {
        IImage? icon = null;

        if (Icon != null)
        {
            var stream = AssetLoader.Open(Icon);

            if (Icon.AbsolutePath.EndsWith(".svg"))
            {
                icon = new SvgImage() { Source = SvgSource.LoadFromStream(stream) };
            }
            else
            {
                icon = new Bitmap(stream);
            }
            stream.Dispose();
        }
        return new Avalonia.Controls.MenuItem() { Header = Header, Command = Command, CommandParameter = CommandParameter, Icon = new Image() { Width = 16, Height = 16, Source = icon}, ItemsSource = Items != null && Items.Count > 0 ? Items.Select(i => i.ToAvaloniaMenuItem()) : null };
    }
}
