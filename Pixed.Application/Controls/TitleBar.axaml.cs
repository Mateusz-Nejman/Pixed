using Avalonia;
using Avalonia.Controls;
using System.Collections.Generic;

namespace Pixed.Application.Controls;

internal partial class TitleBar : EmptyExtendedControl
{
    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public bool CanMinimize
    {
        get { return GetValue(CanMinimizeProperty); }
        set { SetValue(CanMinimizeProperty, value); }
    }

    public bool CanMaximize
    {
        get { return GetValue(CanMaximizeProperty); }
        set { SetValue(CanMaximizeProperty, value); }
    }

    public List<MenuItem>? Menu
    {
        get => GetValue(MenuProperty);
        set => SetValue(MenuProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<TitleBar, string>("Title", "Title");
    public static readonly StyledProperty<bool> CanMinimizeProperty = AvaloniaProperty.Register<TitleBar, bool>("CanMinimize", true);
    public static readonly StyledProperty<bool> CanMaximizeProperty = AvaloniaProperty.Register<TitleBar, bool>("CanMaximize", true);
    public static readonly StyledProperty<List<MenuItem>?> MenuProperty = AvaloniaProperty.Register<TitleBar, List<MenuItem>?>(nameof(Menu), null);
    public TitleBar() : base()
    {
        InitializeComponent();
    }
}