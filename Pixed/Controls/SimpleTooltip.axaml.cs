using Avalonia;
using Avalonia.Controls;

namespace Pixed.Controls;

internal partial class SimpleTooltip : PixedUserControl
{
    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<SimpleTooltip, string>("Title", "Title");
    public SimpleTooltip()
    {
        InitializeComponent();
    }
}
