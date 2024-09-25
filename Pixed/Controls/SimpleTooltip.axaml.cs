using Avalonia;

namespace Pixed.Controls;

internal partial class SimpleTooltip : EmptyPixedUserControl
{
    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<SimpleTooltip, string>("Title", "Title");
    public SimpleTooltip() : base()
    {
        InitializeComponent();
    }
}
