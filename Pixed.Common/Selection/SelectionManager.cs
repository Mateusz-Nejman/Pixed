using Avalonia.Input;
using Pixed.Common.Models;
using Pixed.Common.Platform;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Common.Tools.Selection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pixed.Common.Selection;

public class SelectionManager
{
    private readonly ApplicationData _applicationData;
    private readonly ToolSelector _toolSelector;
    private readonly IClipboardHandle _clipboardHandle;
    private BaseSelection? _currentSelection;

    public bool HasSelection => _currentSelection != null;
    public BaseSelection? Selection => _currentSelection;

    public SelectionManager(ApplicationData applicationData, ShortcutService shortcutService, ToolSelector toolSelector, IClipboardHandle clipboardHandle)
    {
        _applicationData = applicationData;
        _toolSelector = toolSelector;
        _currentSelection = null;
        Subjects.SelectionCreating.Subscribe(OnSelectionCreated);
        Subjects.SelectionDismissed.Subscribe(OnSelectionDismissed);
        shortcutService.Add(new KeyState(Key.C, false, true, false), async () => await Copy());
        shortcutService.Add(new KeyState(Key.X, false, true, false), async () => await Cut());
        shortcutService.Add(new KeyState(Key.V, false, true, false), async () => await Paste());
        shortcutService.Add(new KeyState(Key.A, false, true, false), SelectAll);
        shortcutService.Add(new KeyState(Key.Delete, false, false, false), Erase);
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
            await CopyToClipboard(selectionBitmap);
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
        SKBitmap? source = await CreateBitmapFromClipboard();

        if (source == null)
        {
            return;
        }

        Point startPosition = new(0);

        if (_currentSelection != null)
        {
            int selectionX = _currentSelection.Pixels.Min(p => p.Position.X);
            int selectionY = _currentSelection.Pixels.Min(p => p.Position.Y);
            startPosition = new Point(selectionX, selectionY);
        }

        Frame frame = _applicationData.CurrentFrame;
        List<Pixel> pixels = [];

        for (int x = 0; x < source.Width; x++)
        {
            for (int y = 0; y < source.Height; y++)
            {
                if (frame.ContainsPixel(startPosition + new Point(x, y)))
                {
                    var color = source.GetPixel(x, y);
                    pixels.Add(new Pixel(startPosition + new Point(x, y), (UniColor)color));
                }
            }
        }

        frame.SetPixels(pixels);
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
        _applicationData.CurrentModel.AddHistory();
    }

    private bool IsSelectToolActive()
    {
        return _toolSelector.ToolSelected is ToolSelectBase;
    }

    private async Task CopyToClipboard(SKBitmap src)
    {
        DataObject clipboardObject = new();
        MemoryStream memoryStream = new();
        src.Encode(memoryStream, SKEncodedImageFormat.Png, 100);
        clipboardObject.Set("PNG", memoryStream.ToArray());
        memoryStream.Dispose();
        await _clipboardHandle.ClearAsync();
        await _clipboardHandle.SetDataObjectAsync(clipboardObject);
    }

    private async Task<SKBitmap?> CreateBitmapFromClipboard()
    {
        var formats = await _clipboardHandle.GetFormatsAsync();

        if (formats.Contains("PNG"))
        {
            var data = await _clipboardHandle.GetDataAsync("PNG");

            if (data is byte[] array)
            {
                return SKBitmap.Decode(array);
            }
        }

        return null;
    }
}