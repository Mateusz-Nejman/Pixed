using Avalonia;

namespace Pixed.Controls;

internal partial class TooltipWith2Buttons : PixedUserControl
{
    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public string ButtonText1
    {
        get { return GetValue(ButtonText1Property); }
        set { SetValue(ButtonText1Property, value); }
    }

    public string ButtonTextHelper1
    {
        get { return GetValue(ButtonTextHelper1Property); }
        set { SetValue(ButtonTextHelper1Property, value); }
    }

    public string ButtonText2
    {
        get { return GetValue(ButtonText2Property); }
        set { SetValue(ButtonText2Property, value); }
    }

    public string ButtonTextHelper2
    {
        get { return GetValue(ButtonTextHelper2Property); }
        set { SetValue(ButtonTextHelper2Property, value); }
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<TooltipWith2Buttons, string>("Title", "Title");
    public static readonly StyledProperty<string> ButtonText1Property = AvaloniaProperty.Register<TooltipWith2Buttons, string>("ButtonText1", "ButtonText1");
    public static readonly StyledProperty<string> ButtonTextHelper1Property = AvaloniaProperty.Register<TooltipWith2Buttons, string>("ButtonTextHelper1", "ButtonTextHelper1");
    public static readonly StyledProperty<string> ButtonText2Property = AvaloniaProperty.Register<TooltipWith2Buttons, string>("ButtonText2", "ButtonText2");
    public static readonly StyledProperty<string> ButtonTextHelper2Property = AvaloniaProperty.Register<TooltipWith2Buttons, string>("ButtonTextHelper2", "ButtonTextHelper2");
    public TooltipWith2Buttons()
    {
        InitializeComponent();
    }
}