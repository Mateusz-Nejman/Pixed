using Pixed.Models;
using Pixed.Tools.Selection;
using System;
using System.Collections.Generic;

namespace Pixed.Tools;

internal class ToolSelector
{
    private readonly Dictionary<string, BaseTool> _tools;
    private BaseTool? _toolSelected;

    public Action<string>? SelectToolAction { get; set; } //TODO
    public BaseTool? ToolSelected
    {
        get => _toolSelected;
        set
        {
            var prev = _toolSelected;
            _toolSelected = value;
            Subjects.ToolChanged.OnNext(new BaseToolPair(prev, _toolSelected));
        }
    }

    public ToolSelector(
        ToolPen toolPen, ToolVerticalPen toolVerticalPen, ToolBucket toolBucket, ToolColorSwap toolColorSwap, ToolEraser toolEraser,
        ToolStroke toolStroke, ToolRectangle toolRectangle, ToolCircle toolCircle, ToolMove toolMove, ToolLighten toolLighten,
        ToolDithering toolDithering, ToolColorPicker toolColorPicker, ToolNoise toolNoise, ToolNoiseFill toolNoiseFill,
        ToolOutliner toolOutliner, ToolMoveCanvas toolMoveCanvas, ToolZoom toolZoom, ApplicationData applicationData)
    {
        _tools = new Dictionary<string, BaseTool>()
            {
            { "tool_move_canvas", toolMoveCanvas },
            { "tool_zoom", toolZoom },
            { "tool_pen", toolPen},
            { "tool_mirror_pen", toolVerticalPen},
            { "tool_paint_bucket", toolBucket},
            { "tool_colorswap", toolColorSwap},
            { "tool_eraser", toolEraser},
            { "tool_stroke", toolStroke},
            { "tool_rectangle", toolRectangle},
            { "tool_circle", toolCircle},{ "tool_move", toolMove},
            { "tool_shape_select", new ShapeSelect(applicationData, this)},
            { "tool_rectangle_select", new RectangleSelect(applicationData, this)}, //TODO find better way
            { "tool_lasso_select", new LassoSelect(applicationData, this)},
            { "tool_lighten", toolLighten},
            { "tool_dithering", toolDithering},
            { "tool_colorpicker", toolColorPicker},
            { "tool_noise", toolNoise},
            { "tool_noise_fill", toolNoiseFill},
            { "tool_outliner", toolOutliner}
        };
    }

    public Dictionary<string, BaseTool> GetTools()
    {
        return _tools;
    }

    public void SelectTool(string name)
    {
        if (_tools.ContainsKey(name))
        {
            SelectToolAction?.Invoke(name);
        }
    }
    public BaseTool? GetTool(string name)
    {
        _tools.TryGetValue(name, out BaseTool? value);
        return value;
    }
}
