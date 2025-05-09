using Pixed.Common.Input;
using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
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

    protected void ApplyToolBase(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        _toolColor ??= GetToolColor();
    }

    public virtual void ApplyTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        ApplyToolBase(point, frame, keyState, selection);
    }

    protected void MoveToolBase(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        //TODO highlight cursor
    }

    public virtual void MoveTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        MoveToolBase(point, frame, keyState, selection);
    }

    protected void ReleaseToolBase(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        _toolColor = null;
    }

    public virtual void ReleaseTool(Point point, Frame frame, KeyState keyState, BaseSelection? selection)
    {
        ReleaseToolBase(point, frame, keyState, selection);
    }

    public virtual void OnOverlay(SKCanvas canvas)
    {

    }

    public virtual void Reset()
    {

    }

    public virtual void Initialize()
    {

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

    protected static void SetPixels(Layer layer, List<Pixel> pixels)
    {
        layer.SetPixels(pixels);
        Subjects.LayerModified.OnNext(layer);
    }
}
