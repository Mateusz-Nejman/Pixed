﻿using Avalonia;

namespace Pixed.Controls;

internal partial class TooltipWithButton : EmptyPixedUserControl
{
    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public string ButtonText
    {
        get { return GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public string ButtonTextHelper
    {
        get { return GetValue(ButtonTextHelperProperty); }
        set { SetValue(ButtonTextHelperProperty, value); }
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<TooltipWithButton, string>("Title", "Title");
    public static readonly StyledProperty<string> ButtonTextProperty = AvaloniaProperty.Register<TooltipWithButton, string>("ButtonText", "ButtonText");
    public static readonly StyledProperty<string> ButtonTextHelperProperty = AvaloniaProperty.Register<TooltipWithButton, string>("ButtonTextHelper", "ButtonTextHelper");
    public TooltipWithButton() : base()
    {
        InitializeComponent();
    }
}
