using Pixed.Tools.Selection;
using System;
using System.Collections.Generic;

namespace Pixed.Tools;

internal class ToolSelector
{
    private readonly Dictionary<string, BaseTool> _tools;
    private readonly Action<string> _selectUIToolAction;

    public ToolSelector(Action<string> selectToolAction)
    {
        _selectUIToolAction = selectToolAction;
        _tools = [];
        _tools.Add("tool_pen", new ToolPen());
        _tools.Add("tool_mirror_pen", new ToolVerticalPen());
        _tools.Add("tool_paint_bucket", new ToolBucket());
        _tools.Add("tool_colorswap", new ToolColorSwap());
        _tools.Add("tool_eraser", new ToolEraser());
        _tools.Add("tool_stroke", new ToolStroke());
        _tools.Add("tool_rectangle", new ToolRectangle());
        _tools.Add("tool_circle", new ToolCircle());
        _tools.Add("tool_move", new ToolMove());
        _tools.Add("tool_shape_select", new ShapeSelect());
        _tools.Add("tool_rectangle_select", new RectangleSelect());
        _tools.Add("tool_lasso_select", new LassoSelect());
        _tools.Add("tool_lighten", new ToolLighten());
        _tools.Add("tool_dithering", new ToolDithering());
        _tools.Add("tool_colorpicker", new ToolColorPicker());
        _tools.Add("tool_noise", new ToolNoise());
    }

    public void SelectTool(string name)
    {
        if (_tools.ContainsKey(name))
        {
            _selectUIToolAction?.Invoke(name);
        }
    }
    public BaseTool? GetTool(string name)
    {
        _tools.TryGetValue(name, out BaseTool? value);
        return value;
    }
}
