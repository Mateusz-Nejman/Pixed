using Pixed.Input;
using Pixed.Models;
using Pixed.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace Pixed.Tools;

internal abstract class BaseTool(ApplicationData applicationData)
{
    protected readonly ApplicationData _applicationData = applicationData;
    protected int _highlightedX = 0;
    protected int _highlightedY = 0;

    public virtual bool AddToHistory { get; protected set; } = true;
    public virtual bool ShiftHandle { get; protected set; } = false;
    public virtual bool ControlHandle { get; protected set; } = false;
    public virtual bool AltHandle { get; protected set; } = false;
    public virtual bool SingleHighlightedPixel { get; protected set; } = false;

    public virtual UniColor GetToolColor()
    {
        if (Mouse.RightButton == MouseButtonState.Pressed)
        {
            return _applicationData.SecondaryColor;
        }

        return _applicationData.PrimaryColor;
    }

    public virtual void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {

    }

    public virtual void MoveTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {

    }

    public virtual void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {

    }

    public virtual void Reset()
    {

    }

    public virtual void UpdateHighlightedPixel(int x, int y, Frame frame, ref Bitmap overlay)
    {
        overlay ??= new Bitmap(frame.Width, frame.Height);

        if (_highlightedX != x || _highlightedY != y)
        {
            overlay?.Clear();
        }

        _highlightedX = x;
        _highlightedY = y;

        int pixel = frame.GetPixel(x, y);

        if (overlay.ContainsPixel(x, y))
        {
            overlay.SetPixel(x, y, GetHighlightColor(pixel), SingleHighlightedPixel ? 1 : _applicationData.ToolSize);
        }
    }

    protected static void SetPixels(Frame frame, List<Pixel> pixels)
    {
        frame.SetPixels(pixels);
        Subjects.FrameModified.OnNext(frame);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
    }

    protected static void SetPixels(Layer layer, List<Pixel> pixels)
    {
        layer.SetPixels(pixels);
        Subjects.LayerModified.OnNext(layer);
    }

    private static UniColor GetHighlightColor(int pixel)
    {
        UniColor.Hsl hsl = ((UniColor)pixel).ToHsl();

        if (hsl.L > 0.5)
        {
            return UniColor.WithAlpha(50, UniColor.Black);
        }

        return UniColor.WithAlpha(50, UniColor.White);
    }
}
