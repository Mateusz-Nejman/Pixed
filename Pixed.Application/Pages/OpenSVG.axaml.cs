using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using Pixed.Core.Models;
using Svg.Skia;
using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Pages;

internal partial class OpenSVG : Modal
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

    public static readonly StyledProperty<int> WidthValueProperty = AvaloniaProperty.Register<OpenSVG, int>("WidthValue");
    public static readonly StyledProperty<int> HeightValueProperty = AvaloniaProperty.Register<OpenSVG, int>("HeightValue");
    public OpenSVG()
    {
        InitializeComponent();
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        if (args is SKSvg svg && svg.Picture != null)
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