using Avalonia.Input;
using Pixed.Models;
using Pixed.Tools.Selection;
using Pixed.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Point = System.Drawing.Point;

namespace Pixed.Selection;

internal class SelectionManager
{
    private BaseSelection? _currentSelection;
    private readonly Action<Bitmap> _setOverlayAction;

    public bool HasSelection => _currentSelection != null;
    public BaseSelection? Selection => _currentSelection;

    public SelectionManager(Action<Bitmap> overlayAction)
    {
        _setOverlayAction = overlayAction;
        _currentSelection = null;

        Subjects.ClipboardCopy.Subscribe(_ => Copy());
        Subjects.ClipboardCut.Subscribe(_ => Cut());
        Subjects.ClipboardPaste.Subscribe(_ => Paste());
        Subjects.SelectionCreated.Subscribe(OnSelectionCreated);
        Subjects.SelectionDismissed.Subscribe(OnSelectionDismissed);
        Global.ShortcutService?.Add(new Services.Keyboard.KeyState(Key.C, false, true, false), Copy);
        Global.ShortcutService?.Add(new Services.Keyboard.KeyState(Key.X, false, true, false), Cut);
        Global.ShortcutService?.Add(new Services.Keyboard.KeyState(Key.V, false, true, false), () => Paste());
        Global.ShortcutService?.Add(new Services.Keyboard.KeyState(Key.A, false, true, false), SelectAll);
        Global.ShortcutService?.Add(new Services.Keyboard.KeyState(Key.Delete, false, false, false), Erase);
    }

    private void Clear()
    {
        if (_currentSelection != null)
        {
            _currentSelection.Reset();
            _currentSelection = null;
        }
    }

    private void SelectAll()
    {
        Global.ToolSelected = Global.ToolSelector.GetTool("tool_rectangle_select");
        Subjects.ToolChanged.OnNext(Global.ToolSelected);
        ((RectangleSelect)Global.ToolSelected).SelectAll(_setOverlayAction);
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
        var frame = Global.CurrentFrame;

        for (int a = 0; a < pixels.Count; a++)
        {
            var p = pixels[a];
            int newColor = UniColor.Transparent;

            frame.SetPixel(p.X, p.Y, newColor);
        }

        Subjects.FrameModified.OnNext(frame);
        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
        Global.CurrentModel.AddHistory();
    }

    private void Copy()
    {
        if (!IsSelectToolActive())
        {
            return;
        }

        if (_currentSelection != null && Global.CurrentFrame != null)
        {
            _currentSelection.FillSelectionFromFrame(Global.CurrentFrame);
            Bitmap selectionBitmap = _currentSelection.ToBitmap();
            selectionBitmap?.CopyToClipboard();
        }
    }

    private void Cut()
    {
        if (!IsSelectToolActive())
        {
            return;
        }
        Copy();
        Erase();
    }

    private async Task Paste()
    {
        Global.ToolSelected = Global.ToolSelector.GetTool("tool_rectangle_select");
        Subjects.ToolChanged.OnNext(Global.ToolSelected);
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

        Frame frame = Global.CurrentFrame;

        for (int x = 0; x < source.Width; x++)
        {
            for (int y = 0; y < source.Height; y++)
            {
                if (frame.ContainsPixel(startPosition.X + x, startPosition.Y + y))
                {
                    var color = source.GetPixel(x, y);
                    frame.SetPixel(startPosition.X + x, startPosition.Y + y, color.ToArgb());
                }
            }
        }

        Subjects.LayerModified.OnNext(frame.CurrentLayer);
        Subjects.FrameModified.OnNext(frame);
        Global.CurrentModel.AddHistory();
    }

    private static bool IsSelectToolActive()
    {
        return Global.ToolSelected is BaseSelect;
    }
}
