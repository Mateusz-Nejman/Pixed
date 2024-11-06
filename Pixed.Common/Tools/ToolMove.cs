using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolMove(ApplicationData applicationData) : BaseTool(applicationData)
{
    private const string PROP_WRAP = "Wrap canvas borders";
    private Point _start = new(-1);
    private Layer _currentLayer;
    private Layer _currentLayerClone;

    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-move.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Move content", "Ctrl", "Apply to all layers", "Shift", "Apply to all frames", "Alt", "Wrap canvas borders");
    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override bool AltHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;

    public override void ApplyTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        ApplyToolBase(point, frame, ref overlay, keyState);
        _start = point;
        _currentLayer = frame.CurrentLayer;
        _currentLayerClone = _currentLayer.Clone();
    }

    public override void MoveTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        var diff = point - _start;

        ShiftLayer(frame.CurrentLayer, _currentLayerClone, diff, keyState.IsAlt);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
    }

    public override void ReleaseTool(Point point, Frame frame, ref SKBitmap overlay, KeyState keyState)
    {
        var shiftPressed = keyState.IsShift || GetProperty(ToolProperties.PROP_APPLY_ALL_FRAMES);
        var controlPressed = keyState.IsCtrl || GetProperty(ToolProperties.PROP_APPLY_ALL_LAYERS);
        var altPressed = keyState.IsAlt || GetProperty(PROP_WRAP);
        var diff = point - _start;

        _applicationData.CurrentModel.Process(shiftPressed, controlPressed, (frame, layer) =>
        {
            var reference = _currentLayer == layer ? _currentLayerClone : layer.Clone();
            ShiftLayer(layer, reference, diff, altPressed);
        }, _applicationData, true);

        ReleaseToolBase(point, frame, ref overlay, keyState);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            ToolProperties.GetApplyToAllLayers(),
            ToolProperties.GetApplyToAllFrames(),
            new ToolProperty(PROP_WRAP)
        ];
    }

    private static void ShiftLayer(Layer layer, Layer reference, Point diff, bool altPressed)
    {
        uint color;

        List<Pixel> pixels = [];
        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                var point = new Point(x, y) - diff;

                if (altPressed)
                {
                    point.X = (point.X + layer.Width) % layer.Width;
                    point.Y = (point.Y + layer.Height) % layer.Height;
                }

                if (reference.ContainsPixel(point))
                {
                    color = reference.GetPixel(point);
                }
                else
                {
                    color = UniColor.Transparent;
                }

                pixels.Add(new Pixel(new Point(x, y), color));
            }
        }

        SetPixels(layer, pixels);
    }
}
