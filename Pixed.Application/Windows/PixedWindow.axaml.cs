using Avalonia;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace Pixed.Application.Windows;

public partial class PixedWindow : Page
{
    public object? Dialog
    {
        get { return GetValue(DialogProperty); }
        set { SetValue(DialogProperty, value); }
    }

    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly StyledProperty<object?> DialogProperty = AvaloniaProperty.Register<PixedWindow, object?>(nameof(Dialog), null);
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<PixedWindow, string>(nameof(Title), "Title");
    public PixedWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Navigator?.BackAsync();
    }
}

