using Pixed.Input;
using Pixed.Models;
using Pixed.Utils;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Tools;

internal abstract class BaseTool(ApplicationData applicationData)
{
    private readonly List<ToolProperty> _properties = [];
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

    public virtual void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {

    }

    public virtual void MoveTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {

    }

    public virtual void ReleaseTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {

    }

    public virtual void Reset()
    {

    }

    public virtual void UpdateHighlightedPixel(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        overlay ??= new SKBitmap(frame.Width, frame.Height, true);

        if (_highlightedX != x || _highlightedY != y)
        {
            overlay?.Clear();
        }

        _highlightedX = x;
        _highlightedY = y;

        uint pixel = frame.GetPixel(x, y);

        if (overlay.ContainsPixel(x, y))
        {
            overlay.SetPixel(x, y, GetHighlightColor(pixel), SingleHighlightedPixel ? 1 : _applicationData.ToolSize);
        }
    }

    public virtual List<ToolProperty> GetToolProperties()
    {
        return [];
    }

    public void ResetProperties()
    {
        _properties.Clear();
        _properties.AddRange(GetToolProperties());
    }

    public List<ToolProperty> GetCurrentProperties()
    {
        var toolProperties = GetToolProperties();

        if (toolProperties.Count != _properties.Count)
        {
            ResetProperties();
        }
        return _properties;
    }

    public bool GetProperty(string name)
    {
        return _properties.FirstOrDefault(p => p.Name == name, null)?.Checked == true;
    }

    public void SetProperty(string name, bool value)
    {
        if (!_properties.Exists(p => p.Name == name))
        {
            return;
        }

        int index = _properties.FindIndex(p => p.Name == name);
        _properties[index].Checked = value;
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

    private static UniColor GetHighlightColor(uint pixel)
    {
        UniColor.Hsl hsl = ((UniColor)pixel).ToHsl();

        if (hsl.L > 0.5)
        {
            return UniColor.WithAlpha(50, UniColor.Black);
        }

        return UniColor.WithAlpha(50, UniColor.White);
    }
}
