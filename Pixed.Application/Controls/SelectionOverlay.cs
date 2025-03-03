using Pixed.Application.Utils;
using Pixed.Core;
using Pixed.Core.Algorithms;
using Pixed.Core.Selection;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Pixed.Application.Controls;
internal class SelectionOverlay : OverlayControl
{
    private List<Tuple<SKPoint, SKPoint>> _lines = [];
    private BaseSelection? _selection;
    private double _zoom = 1.0f;
    private bool _drawLines = false;
    private float[] _pattern = [2f, 2f];

    public override double Zoom
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

            if (!value)
            {
                _lines.Clear();
            }
        }
    }

    public void UpdateSelection(BaseSelection? selection)
    {
        _selection = selection;

        if (DrawLines)
        {
            UpdateLines();
        }
    }
    public override void Render(SKCanvas canvas)
    {
        if (_selection == null) return;

        if (DrawLines)
        {
            foreach (var line in _lines.ToArray())
            {
                canvas.DrawLine(line.Item1, line.Item2, UniColor.Black);
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
        if (_selection == null)
        {
            return;
        }

        if (_selection.Pixels.Count == 0)
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

            var color = new UniColor(128, 12, 84, 139);
            var hsl = color.ToHsl();
            color = pixel.Color;

            if (hsl.L < 0.5d)
            {
                color = color.Darken(10);

            }
            else
            {
                color = color.Lighten(10);
            }

            color.A = 128;

            var paint = new SKPaint()
            {
                Color = color
            };

            canvas.DrawRect(SKRect.Create(pixel.Position.X, pixel.Position.Y, 1f, 1f), paint);
        }
    }
}