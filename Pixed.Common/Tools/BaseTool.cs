using Pixed.Common.Input;
using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Pixed.Common.Tools;

public abstract class BaseTool(ApplicationData applicationData)
{
    private readonly List<ToolProperty> _properties = [];
    protected readonly ApplicationData _applicationData = applicationData;
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

    protected void ToolBeginBase()
    {
        _toolColor ??= GetToolColor();
    }

    public virtual void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase();
    }

    public virtual void ToolMove(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
    }

    protected void ToolEndBase()
    {
        _toolColor = null;
    }

    public virtual void ToolEnd(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolEndBase();
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
        return _properties.FirstOrDefault(p => p != null && p.Name == name, null)?.Checked == true;
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
}
