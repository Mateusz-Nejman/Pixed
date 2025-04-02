using Pixed.Common.Input;
using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools;

public abstract class BaseTool(ApplicationData applicationData)
{
    private readonly List<ToolProperty> _properties = [];
    protected readonly ApplicationData _applicationData = applicationData;
    protected Point _highlightedPoint = new();
    private UniColor? _toolColor;

    public abstract string ImagePath { get; }
    public abstract string Name { get; }
    public abstract string Id { get; }
    public virtual ToolTooltipProperties? ToolTipProperties { get; }
    public virtual bool AddToHistory { get; protected set; } = true;
    public virtual bool SingleHighlightedPixel { get; protected set; } = false;

    protected UniColor ToolColor => _toolColor ?? _applicationData.PrimaryColor;

    public virtual UniColor GetToolColor()
    {
        if (Mouse.RightButton == MouseButtonState.Pressed)
        {
            return _applicationData.SecondaryColor;
        }

        return _applicationData.PrimaryColor;
    }

    protected void ApplyToolBase(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        _toolColor ??= GetToolColor();
    }

    public virtual void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
    }

    public virtual void MoveTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {

    }

    protected void ReleaseToolBase(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        _toolColor = null;
    }

    public virtual void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ReleaseToolBase(point, frame, ref overlay, keyState);
    }

    public virtual void Reset()
    {

    }

    public virtual void Initialize()
    {

    }

    public virtual void UpdateHighlightedPixel(Point point, Frame frame, ref SKBitmap overlay)
    {
        overlay ??= new SKBitmap(frame.Width, frame.Height, true);

        if (_highlightedPoint != point)
        {
            overlay?.Clear();
        }

        _highlightedPoint = point;

        uint pixel = frame.GetPixel(point);

        if (overlay.ContainsPixel(point))
        {
            overlay.SetPixel(point, GetHighlightColor(pixel), SingleHighlightedPixel ? 1 : _applicationData.ToolSize);
        }

        Subjects.OverlayModified.OnNext(overlay);
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
