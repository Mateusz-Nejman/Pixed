using Avalonia.Input;
using Pixed.BigGustave;
using Pixed.Common.Platform;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools;
using Pixed.Common.Tools.Selection;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
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
        _clipboardHandle = clipboardHandle;
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
        _toolSelector.SelectTool("tool_rectangle_select");
        var selection = ToolSelectRectangle.Create(new Point(), new Point(_applicationData.CurrentModel.Width - 1, _applicationData.CurrentModel.Height - 1), _applicationData.CurrentFrame);
        Subjects.SelectionCreating.OnNext(selection);
        Subjects.SelectionCreated.OnNext(selection);
        Subjects.OverlayModified.OnNext(null);
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
            await CopySelectionAsync(_currentSelection);
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

    private async Task CopySelectionAsync(BaseSelection selection)
    {
        var minX = selection.Pixels.Min(p => p.Position.X);
        var minY = selection.Pixels.Min(p => p.Position.Y);
        var maxX = selection.Pixels.Max(p => p.Position.X);
        var maxY = selection.Pixels.Max(p => p.Position.Y);
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;
        var builder = PngBuilder.Create(width, height, true);

        foreach (var pixel in selection.Pixels)
        {
            builder.SetPixel(new UniColor(pixel.Color), pixel.Position.X - minX, pixel.Position.Y - minY);
        }

        MemoryStream memoryStream = new();
        builder.Save(memoryStream);
        DataObject clipboardObject = new();
        clipboardObject.Set("PNG", memoryStream.ToArray());
        await _clipboardHandle.ClearAsync();
        await _clipboardHandle.SetDataObjectAsync(clipboardObject);
        memoryStream.Dispose();
    }
}