using Pixed.Models;
using SkiaSharp;
using System.Collections.Generic;

namespace Pixed.Tools;
internal class ToolMove(ApplicationData applicationData) : BaseTool(applicationData)
{
    private const string PROP_WRAP = "Wrap canvas borders";
    private int _startX = -1;
    private int _startY = -1;
    private Layer _currentLayer;
    private Layer _currentLayerClone;

    public override bool ShiftHandle { get; protected set; } = true;
    public override bool ControlHandle { get; protected set; } = true;
    public override bool AltHandle { get; protected set; } = true;
    public override bool SingleHighlightedPixel { get; protected set; } = true;

    public override void ApplyTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        base.ApplyTool(x, y, frame, ref overlay, shiftPressed, controlPressed, altPressed);
        _startX = x;
        _startY = y;
        _currentLayer = frame.CurrentLayer;
        _currentLayerClone = _currentLayer.Clone();
    }

    public override void MoveTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        int diffX = x - _startX;
        int diffY = y - _startY;

        ShiftLayer(frame.CurrentLayer, _currentLayerClone, diffX, diffY, altPressed);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
    }

    public override void ReleaseTool(int x, int y, Frame frame, ref SKBitmap overlay, bool shiftPressed, bool controlPressed, bool altPressed)
    {
        shiftPressed = shiftPressed || GetProperty(ToolProperties.PROP_APPLY_ALL_FRAMES);
        controlPressed = controlPressed || GetProperty(ToolProperties.PROP_APPLY_ALL_LAYERS);
        altPressed = altPressed || GetProperty(PROP_WRAP);
        int diffX = x - _startX;
        int diffY = y - _startY;

        _applicationData.CurrentModel.Process(shiftPressed, controlPressed, (frame, layer) =>
        {
            var reference = this._currentLayer == layer ? this._currentLayerClone : layer.Clone();
            ShiftLayer(layer, reference, diffX, diffY, altPressed);
        }, _applicationData, true);

        base.ReleaseTool(x, y, frame, ref overlay, shiftPressed, controlPressed, altPressed);
    }

    public override List<ToolProperty> GetToolProperties()
    {
        return [
            ToolProperties.GetApplyToAllLayers(),
            ToolProperties.GetApplyToAllFrames(),
            new ToolProperty(PROP_WRAP)
        ];
    }

    private static void ShiftLayer(Layer layer, Layer reference, int diffX, int diffY, bool altPressed)
    {
        uint color;

        List<Pixel> pixels = [];
        for (int x = 0; x < layer.Width; x++)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                int x1 = x - diffX;
                int y1 = y - diffY;

                if (altPressed)
                {
                    x1 = (x1 + layer.Width) % layer.Width;
                    y1 = (y1 + layer.Height) % layer.Height;
                }

                if (reference.ContainsPixel(x1, y1))
                {
                    color = reference.GetPixel(x1, y1);
                }
                else
                {
                    color = UniColor.Transparent;
                }

                pixels.Add(new Pixel(x, y, color));
            }
        }

        SetPixels(layer, pixels);
    }
}
