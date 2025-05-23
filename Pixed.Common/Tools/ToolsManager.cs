﻿using Pixed.Common.Models;
using Pixed.Common.Tools.Selection;
using System;
using System.Collections.Generic;

namespace Pixed.Common.Tools;

public class ToolsManager
{
    private readonly Dictionary<string, BaseTool> _tools;
    private BaseTool? _selectedTool;

    public Action<string>? SelectToolAction { get; set; } //TODO
    public BaseTool? SelectedTool
    {
        get => _selectedTool;
        set
        {
            var prev = _selectedTool;
            _selectedTool = value;
            _selectedTool?.Initialize();
            Subjects.ToolChanged.OnNext(new BaseToolPair(prev, _selectedTool));
        }
    }

    public ToolsManager(
        ToolPen toolPen, ToolBucket toolBucket, ToolEraser toolEraser,
        ToolStroke toolStroke, ToolRectangle toolRectangle, ToolCircle toolCircle, ToolLighten toolLighten,
        ToolDithering toolDithering, ToolColorPicker toolColorPicker,
        ToolOutliner toolOutliner, ToolMoveCanvas toolMoveCanvas, ToolSelectShape shapeSelect, ToolSelectRectangle rectangleSelect, ToolSelectLasso lassoSelect)
    {
        _tools = new Dictionary<string, BaseTool>()
            {
            { "tool_move_canvas", toolMoveCanvas },
            { "tool_pen", toolPen},
            { "tool_paint_bucket", toolBucket},
            { "tool_eraser", toolEraser},
            { "tool_stroke", toolStroke},
            { "tool_rectangle", toolRectangle},
            { "tool_circle", toolCircle},
            { "tool_shape_select", shapeSelect},
            { "tool_rectangle_select", rectangleSelect},
            { "tool_lasso_select", lassoSelect},
            { "tool_lighten", toolLighten},
            { "tool_dithering", toolDithering},
            { "tool_colorpicker", toolColorPicker},
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
