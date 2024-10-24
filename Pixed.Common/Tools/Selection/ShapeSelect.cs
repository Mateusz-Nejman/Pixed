﻿using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;
using SkiaSharp;

namespace Pixed.Common.Tools.Selection;

internal class ShapeSelect(ApplicationData applicationData, ToolSelector toolSelector) : AbstractDragSelect(applicationData, toolSelector)
{
    public override string ImagePath => "avares://Pixed.Application/Resources/Icons/tools/tool-shape-select.png";
    public override ToolTooltipProperties? ToolTipProperties => new ToolTooltipProperties("Shape selection", "Ctrl+C", "Copy the selected area", "Ctrl+V", "Paste the copied area");
    public override void OnSelectStart(int x, int y, Frame frame, ref SKBitmap overlay)
    {
        _hasSelection = true;
        var pixels = PaintUtils.GetSimiliarConnectedPixels(frame, x, y);
        _selection = new ShapeSelection(pixels);
        Subjects.SelectionCreated.OnNext(_selection);
        DrawSelectionOnOverlay(ref overlay);
    }
}
