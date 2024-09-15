using Avalonia;
using Avalonia.Controls;

namespace Pixed.Controls;

public partial class SimpleTooltip : UserControl
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
