using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using Pixed.Core.Models;
using System.Threading.Tasks;
using System.Threading;
using Svg.Skia;

namespace Pixed.Application.Windows;

internal partial class OpenSVGWindow : PixedWindow
{
    public int WidthValue
    {
        get => GetValue(WidthValueProperty);
        set => SetValue(WidthValueProperty, value);
    }

    public int HeightValue
    {
        get => GetValue(HeightValueProperty);
        set => SetValue(HeightValueProperty, value);
    }

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<OpenSVGWindow, int>("WidthValue");
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<OpenSVGWindow, int>("HeightValue");
    public OpenSVGWindow()
    {
        InitializeComponent();
        var applicationData = Provider.Get<ApplicationData>();
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        if (args is SKSvg svg)
        {
            WidthValue = (int)svg.Picture.CullRect.Width;
            HeightValue = (int)svg.Picture.CullRect.Height;
        }
        return Task.CompletedTask;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(new OpenSvgResult(WidthValue, HeightValue));
    }
}