using Avalonia.Input;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Tools;
using Pixed.Tools.Selection;
using Pixed.Utils;
using Pixed.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        shortcutService.Add(new Services.Keyboard.KeyState(Key.C, false, true, false), Copy);
        shortcutService.Add(new Services.Keyboard.KeyState(Key.X, false, true, false), Cut);
        shortcutService.Add(new Services.Keyboard.KeyState(Key.V, false, true, false), () => Paste());
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
        _toolSelector.ToolSelected = _toolSelector.GetTool("tool_rectangle_select");
        ((RectangleSelect)_toolSelector.ToolSelected).SelectAll(overlay => _paintCanvas.Overlay = overlay);
    }

    public void Copy()
    {
        if (!IsSelectToolActive())
        {
            return;
        }

        if (_currentSelection != null && _applicationData.CurrentFrame != null)
        {
            _currentSelection.FillSelectionFromFrame(_applicationData.CurrentFrame);
            Bitmap selectionBitmap = _currentSelection.ToBitmap();
            selectionBitmap?.CopyToClipboard();
        }
    }

    public void Cut()
    {
        if (!IsSelectToolActive())
        {
            return;
        }
        Copy();
        Erase();
    }

    public async Task Paste()
    {
        _toolSelector.ToolSelected = _toolSelector.GetTool("tool_rectangle_select");
        Bitmap? source = await BitmapUtils.CreateFromClipboard();

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
                    pixels.Add(new Pixel(startPosition.X + x, startPosition.Y + y, color.ToArgb()));
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

        for (int a = 0; a < pixels.Count; a++)
        {
            pixels[a].Color = UniColor.Transparent;
        }

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
