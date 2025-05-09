using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using Pixed.Core.Models;
using SkiaSharp;

namespace Pixed.Application.Controls.PaintCanvas;

internal class PaintContainer : PixelImageControl
{
    protected override ICustomPixedImageOperation GetOperation()
    {
        return new PaintContainerOperation();
    }
}