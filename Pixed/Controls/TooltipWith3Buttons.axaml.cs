using Avalonia;
using Avalonia.Controls;

namespace Pixed.Controls;

internal partial class TooltipWith3Buttons : PixedUserControl
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

    public string ButtonText3
    {
        get { return GetValue(ButtonText3Property); }
        set { SetValue(ButtonText3Property, value); }
    }

    public string ButtonTextHelper3
    {
        get { return GetValue(ButtonTextHelper3Property); }
        set { SetValue(ButtonTextHelper3Property, value); }
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<TooltipWith3Buttons, string>("Title", "Title");
    public static readonly StyledProperty<string> ButtonText1Property = AvaloniaProperty.Register<TooltipWith3Buttons, string>("ButtonText1", "ButtonText1");
    public static readonly StyledProperty<string> ButtonTextHelper1Property = AvaloniaProperty.Register<TooltipWith3Buttons, string>("ButtonTextHelper1", "ButtonTextHelper1");
    public static readonly StyledProperty<string> ButtonText2Property = AvaloniaProperty.Register<TooltipWith3Buttons, string>("ButtonText2", "ButtonText2");
    public static readonly StyledProperty<string> ButtonTextHelper2Property = AvaloniaProperty.Register<TooltipWith3Buttons, string>("ButtonTextHelper2", "ButtonTextHelper2");
    public static readonly StyledProperty<string> ButtonText3Property = AvaloniaProperty.Register<TooltipWith3Buttons, string>("ButtonText3", "ButtonText3");
    public static readonly StyledProperty<string> ButtonTextHelper3Property = AvaloniaProperty.Register<TooltipWith3Buttons, string>("ButtonTextHelper3", "ButtonTextHelper3");
    public TooltipWith3Buttons()
    {
        InitializeComponent();
    }
}
