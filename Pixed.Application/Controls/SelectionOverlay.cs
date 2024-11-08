using Avalonia.Controls;
using Pixed.Application.Utils;
using Pixed.Common;
using Pixed.Common.Algos;
using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reactive.Joins;

namespace Pixed.Application.Controls;
internal class SelectionOverlay : OverlayControl
{
    private List<Tuple<SKPoint, SKPoint>> _lines = [];
    private BaseSelection? _selection;
    private double _zoom = 1.0f;
    private bool _drawLines = false;
    private float[] _pattern = [2f, 2f];

    public double Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
            _pattern = [2f / _zoom.ToFloat(), 2f / _zoom.ToFloat()];
            UpdateSelection(_selection);
        }
    }

    public bool DrawLines
    {
        get => _drawLines;
        set
        {
            _drawLines = value;

            if(!value)
            {
                _lines.Clear();
            }
        }
    }

    public void UpdateSelection(BaseSelection? selection)
    {
        _selection = selection;

        if(DrawLines)
        {
            UpdateLines();
        }
    }
    public override void Render(SKCanvas canvas)
    {
        if(_selection == null) return;

        if(DrawLines)
        {
            foreach (var line in _lines.ToArray())
            {
                canvas.DrawPatternLine(line.Item1, line.Item2, _pattern, UniColor.White);
            }
        }
        else
        {
            DrawSelectionBackground(canvas, _selection);
        }
    }

    private void UpdateLines()
    {
        if(_selection == null)
        {
            return;
        }

        if(_selection.Pixels.Count == 0)
        {
            _lines.Clear();
            return;
        }

        _lines = SelectionBorder.Get(_selection);
    }

    private static void DrawSelectionBackground(SKCanvas canvas, BaseSelection selection)
    {
        var pixels = selection.Pixels;

        for (int i = 0; i < pixels.Count; i++)
        {
            var pixel = pixels[i];
            var hasColor = pixel.Color != UniColor.Transparent;

            var color = new UniColor(128, 12, 84, 139);

            if (hasColor)
            {
                color = pixel.Color;
                color = color.Lighten(10);
                color.A = 128;
            }

            var paint = new SKPaint()
            {
                Color = color
            };

            canvas.DrawRect(SKRect.Create(pixel.Position.X, pixel.Position.Y, 1f, 1f), paint);
        }
    }
}