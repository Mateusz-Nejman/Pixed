using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using ColorPicker.Models;
using System;

namespace Pixed.Controls;

public partial class FixedPortableColorPicker : UserControl
{
    public static readonly StyledProperty<Color> SelectedColorProperty =
        AvaloniaProperty.Register<FixedPortableColorPicker, Color>(
            nameof(SelectedColor), Colors.Transparent, coerce: (o, color) =>
            {
                if (o is FixedPortableColorPicker picker)
                {
                    picker.standardColorPicker.SelectedColor = color;
                    picker.toggleBorder.Background = new SolidColorBrush(color);
                }
                return color;
            });

    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<FixedPortableColorPicker, bool>(
            nameof(IsChecked), false);
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
    public Color SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
    public FixedPortableColorPicker()
    {
        InitializeComponent();
        standardColorPicker.Color.PropertyChanged += Color_PropertyChanged;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var popupPart = e.NameScope.Find<Popup>("popup");
        if (popupPart != null)
        {
            popupPart.PointerPressed += (sender, args) =>
            {
                args.Handled = true;
            };
        }
    }

    private void Color_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is NotifyableColor notifyableColor)
        {
            var color = Color.FromArgb(
                (byte)Math.Round(notifyableColor.A),
                (byte)Math.Round(notifyableColor.RGB_R),
                (byte)Math.Round(notifyableColor.RGB_G),
                (byte)Math.Round(notifyableColor.RGB_B));

            SelectedColor = color;
        }
    }

    private void Border_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        IsChecked = !IsChecked;
    }
}