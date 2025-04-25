using Avalonia.Input;
using Pixed.Common.Services;
using Pixed.Common.Services.Keyboard;
using Pixed.Common.Tools.Selection;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixed.Common.Tools;

public class SelectionManager
{
    private readonly ApplicationData _applicationData;
    private readonly ToolsManager _toolSelector;
    private readonly ClipboardService _clipboardService;
    private BaseSelection? _currentSelection;

    public bool HasSelection => _currentSelection != null;
    public BaseSelection? Selection => _currentSelection;

    public SelectionManager(ApplicationData applicationData, ShortcutService shortcutService, ToolsManager toolSelector, ClipboardService clipboardService)
    {
        _applicationData = applicationData;
        _toolSelector = toolSelector;
        _currentSelection = null;
        _clipboardService = clipboardService;
        Subjects.SelectionCreating.Subscribe(OnSelectionCreated);
        Subjects.SelectionDismissed.Subscribe(OnSelectionDismissed);
        shortcutService.Add(KeyState.Control(Key.C), async () => await Copy());
        shortcutService.Add(KeyState.Control(Key.X), async () => await Cut());
        shortcutService.Add(KeyState.Control(Key.V), async () => await Paste());
        shortcutService.Add(KeyState.Control(Key.A), SelectAll);
        shortcutService.Add(new KeyState(Key.Delete, true, false, false, false), Erase);
    }

    public void Clear()
    {
        if (_currentSelection != null)
        {
            _currentSelection.Reset();
            _currentSelection = null;
            Subjects.SelectionDismissed.OnNext(null);
        }
    }

    public void SelectAll()
    {
        _toolSelector.SelectTool("tool_rectangle_select");
        var selection = ToolSelectRectangle.Create(new Point(), new Point(_applicationData.CurrentModel.Width - 1, _applicationData.CurrentModel.Height - 1), _applicationData.CurrentFrame);
        Subjects.SelectionCreating.OnNext(selection);
        Subjects.SelectionCreated.OnNext(selection);
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
            await _clipboardService.Copy(_currentSelection);
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

        if (_toolSelector.SelectedTool != newTool)
        {
            _toolSelector.SelectedTool = newTool;
        }
        SKBitmap? source = await _clipboardService.GetBitmap();

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

        var pixels = _currentSelection.Pixels.Select(p => new Pixel(p.Position)).ToList();
        var frame = _applicationData.CurrentFrame;

        frame.SetPixels(pixels);

        Subjects.FrameModified.OnNext(frame);
        _applicationData.CurrentModel.AddHistory();
    }

    private bool IsSelectToolActive()
    {
        return _toolSelector.SelectedTool is ToolSelectBase;
    }
}