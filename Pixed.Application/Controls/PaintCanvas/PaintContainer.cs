namespace Pixed.Application.Controls.PaintCanvas;

internal class PaintContainer : PixelImageControl
{
    protected override ICustomPixedImageOperation GetOperation()
    {
        return new PaintContainerOperation();
    }
}