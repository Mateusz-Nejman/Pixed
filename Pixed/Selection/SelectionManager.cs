﻿using Avalonia.Input;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Tools;
using Pixed.Tools.Selection;
using Pixed.Utils;
using Pixed.ViewModels;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Point = System.Drawing.Point;

namespace Pixed.Selection;

internal class SelectionManager
{
    private readonly ApplicationData _applicationData;
    private readonly ToolSelector _toolSelector;
    private readonly PaintCanvasViewModel _paintCanvas;
    private BaseSelection? _currentSelection;

    public bool HasSelection => _currentSelection != null;
    public BaseSelection? Selection => _currentSelection;

    public SelectionManager(ApplicationData applicationData, ShortcutService shortcutService, ToolSelector toolSelector, PaintCanvasViewModel paintCanvas)
    {
        _applicationData = applicationData;
        _toolSelector = toolSelector;
        _paintCanvas = paintCanvas;
        _currentSelection = null;

        Subjects.ClipboardCopy.Subscribe(_ => Copy());
        Subjects.ClipboardCut.Subscribe(_ => Cut());
        Subjects.ClipboardPaste.Subscribe(_ => Paste());
        Subjects.SelectionCreated.Subscribe(OnSelectionCreated);
        Subjects.SelectionDismissed.Subscribe(OnSelectionDismissed);
        shortcutService.Add(new Services.Keyboard.KeyState(Key.C, false, true, false), async () => await Copy());
        shortcutService.Add(new Services.Keyboard.KeyState(Key.X, false, true, false), async () => await Cut());
        shortcutService.Add(new Services.Keyboard.KeyState(Key.V, false, true, false), async () => await Paste());
        shortcutService.Add(new Services.Keyboard.KeyState(Key.A, false, true, false), SelectAll);
        shortcutService.Add(new Services.Keyboard.KeyState(Key.Delete, false, false, false), Erase);
    }

    public void Clear()
    {
        if (_currentSelection != null)
        {
            _currentSelection.Reset();
            _currentSelection = null;
        }
    }

    public void SelectAll()
    {
        var newTool = _toolSelector.GetTool("tool_rectangle_select");

        if (_toolSelector.ToolSelected != newTool)
        {
            _toolSelector.ToolSelected = newTool;
        }
        ((RectangleSelect)_toolSelector.ToolSelected).SelectAll(overlay => _paintCanvas.Overlay = overlay);
    }

    public async Task Copy()
    {
        if (!IsSelectToolActive())
        {
            return;
        }

        if (_currentSelection != null && _applicationData.CurrentFrame != null)
        {
            _currentSelection.FillSelectionFromFrame(_applicationData.CurrentFrame);
            SKBitmap selectionBitmap = _currentSelection.ToBitmap();
            await selectionBitmap?.CopyToClipboard();
        }
    }

    public async Task Cut()
    {
        if (!IsSelectToolActive())
        {
            return;
        }
        await Copy();
        Erase();
    }

    public async Task Paste()
    {
        var newTool = _toolSelector.GetTool("tool_rectangle_select");

        if (_toolSelector.ToolSelected != newTool)
        {
            _toolSelector.ToolSelected = newTool;
        }
        SKBitmap? source = await SkiaUtils.CreateFromClipboard();

        if (source == null)
        {
            return;
        }

        Point startPosition = new(0);

        if (_currentSelection != null)
        {
            int selectionX = _currentSelection.Pixels.Min(p => p.X);
            int selectionY = _currentSelection.Pixels.Min(p => p.Y);
            startPosition = new Point(selectionX, selectionY);
        }

        Frame frame = _applicationData.CurrentFrame;
        List<Pixel> pixels = [];

        for (int x = 0; x < source.Width; x++)
        {
            for (int y = 0; y < source.Height; y++)
            {
                if (frame.ContainsPixel(startPosition.X + x, startPosition.Y + y))
                {
                    var color = source.GetPixel(x, y);
                    pixels.Add(new Pixel(startPosition.X + x, startPosition.Y + y, (UniColor)color));
                }
            }
        }

        frame.SetPixels(pixels);

        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
        _applicationData.CurrentModel.AddHistory();
    }

    private void OnSelectionCreated(BaseSelection? selection)
    {
        if (selection != null)
        {
            _currentSelection = selection;
        }
    }

    private void OnSelectionDismissed(BaseSelection? selection)
    {
        Clear();
    }

    private void Erase()
    {
        if (_currentSelection == null || !IsSelectToolActive())
        {
            return;
        }

        var pixels = _currentSelection.Pixels;
        var frame = _applicationData.CurrentFrame;

        frame.SetPixels(pixels);

        Subjects.FrameModified.OnNext(frame);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
        _applicationData.CurrentModel.AddHistory();
    }

    private bool IsSelectToolActive()
    {
        return _toolSelector.ToolSelected is BaseSelect;
    }
}