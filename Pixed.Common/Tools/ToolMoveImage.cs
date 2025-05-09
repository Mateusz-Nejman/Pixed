using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Common.Tools;
public class ToolMoveImage(ApplicationData applicationData) : BaseTool(applicationData)
{
    private const string PROP_WRAP = "Wrap canvas borders";
    private Point _start = new(-1);
    private Layer _currentLayer;
    private Layer _currentLayerClone;

    public override string ImagePath => "avares://Pixed.Application/Resources/fluent-icons/ic_fluent_hand_left_28_regular.svg";
    public override string Name => "Move tool";
    public override string Id => "tool_move";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Move content", "Ctrl", "Apply to all layers", "Shift", "Apply to all frames", "Alt", "Wrap canvas borders");
    public override bool SingleHighlightedPixel { get; protected set; } = true;

    public override void ToolBegin(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolBeginBase(point, model, keyState, selection);
        _start = point;
        _currentLayer = model.CurrentFrame.CurrentLayer;
        _currentLayerClone = _currentLayer.Clone();
    }

    public override void ToolMove(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
    {
        ToolMoveBase(point, model, keyState, selection);
        var layer = model.CurrentFrame.CurrentLayer;
        var diff = point - _start;

        ShiftLayer(layer, _currentLayerClone, diff, keyState.IsAlt);
        Subjects.LayerModified.OnNext(layer);
    }

    public override void ToolEnd(Point point, PixedModel model, KeyState keyState, BaseSelection? selection)
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

        ToolEndBase(point, model, keyState, selection);
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
