using Pixed.Models;
using Pixed.Selection;
using Pixed.Services.Keyboard;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Pixed;

internal static class Subjects
{
    public static Subject<KeyState> KeyState { get; } = new Subject<KeyState>();
    public static Subject<BaseSelection> SelectionCreated { get; } = new Subject<BaseSelection>();
    public static Subject<BaseSelection> SelectionDismissed { get; } = new Subject<BaseSelection>();
    public static Subject<double> MouseWheel { get; } = new Subject<double>();
    public static Subject<bool> GridChanged { get; } = new Subject<bool>();
    public static Subject<BaseToolPair> ToolChanged { get; } = new Subject<BaseToolPair>();
    public static Subject<double> ZoomChanged { get; } = new Subject<double>();
    public static Subject<string[]> NewInstanceHandled { get; } = new Subject<string[]>();
    public static Subject<SKBitmap> OverlayModified { get; } = new Subject<SKBitmap>();
    public static Subject<List<Pixel>> CurrentLayerRenderModified { get; } = new Subject<List<Pixel>>();

    public static Subject<BaseSelection> ClipboardCopy { get; } = new Subject<BaseSelection>();
    public static Subject<BaseSelection> ClipboardCut { get; } = new Subject<BaseSelection>();
    public static Subject<BaseSelection> ClipboardPaste { get; } = new Subject<BaseSelection>();

    public static Subject<UniColor> PrimaryColorChanged { get; } = new Subject<UniColor>();
    public static Subject<UniColor> PrimaryColorChange { get; } = new Subject<UniColor>();
    public static Subject<UniColor> SecondaryColorChanged { get; } = new Subject<UniColor>();
    public static Subject<UniColor> SecondaryColorChange { get; } = new Subject<UniColor>();

    public static Subject<PaletteModel> PaletteAdded { get; } = new Subject<PaletteModel>();
    public static Subject<PaletteModel> PaletteSelected { get; } = new Subject<PaletteModel>();

    public static Subject<PixedModel> ProjectAdded { get; } = new Subject<PixedModel>();
    public static Subject<PixedModel> ProjectChanged { get; } = new Subject<PixedModel>();
    public static Subject<PixedModel> ProjectModified { get; } = new Subject<PixedModel>();
    public static Subject<PixedModel> ProjectRemoved { get; } = new Subject<PixedModel>();

    public static Subject<Frame> FrameAdded { get; } = new Subject<Frame>();
    public static Subject<Frame> FrameModified { get; } = new Subject<Frame>();
    public static Subject<Frame> FrameChanged { get; } = new Subject<Frame>();
    public static Subject<Frame> FrameRemoved { get; } = new Subject<Frame>();

    public static Subject<Layer> LayerAdded { get; } = new Subject<Layer>();
    public static Subject<Layer> LayerModified { get; } = new Subject<Layer>();
    public static Subject<Layer> LayerRemoved { get; } = new Subject<Layer>();
    public static Subject<Layer> LayerChanged { get; } = new Subject<Layer>();

    public static IDisposable[] InitDebug()
    {
        IDisposable[] disposables = [
#if DEBUG
            KeyState.DebugSubscribe(nameof(KeyState)),
            SelectionCreated.DebugSubscribe(nameof(SelectionCreated)),
            SelectionDismissed.DebugSubscribe(nameof(SelectionDismissed)),
            MouseWheel.DebugSubscribe(nameof(MouseWheel)),
            GridChanged.DebugSubscribe(nameof(GridChanged)),
            ToolChanged.DebugSubscribe(nameof(ToolChanged)),
            ZoomChanged.DebugSubscribe(nameof(ZoomChanged)),
            NewInstanceHandled.DebugSubscribe(nameof(NewInstanceHandled)),
            OverlayModified.DebugSubscribe(nameof(OverlayModified)),
            ClipboardCopy.DebugSubscribe(nameof(ClipboardCopy)),
            ClipboardCut.DebugSubscribe(nameof(ClipboardCut)),
            ClipboardPaste.DebugSubscribe(nameof(ClipboardPaste)),
            PrimaryColorChanged.DebugSubscribe(nameof(PrimaryColorChanged)),
            PrimaryColorChange.DebugSubscribe(nameof(PrimaryColorChange)),
            SecondaryColorChanged.DebugSubscribe(nameof(SecondaryColorChanged)),
            SecondaryColorChange.DebugSubscribe(nameof(SecondaryColorChange)),
            PaletteAdded.DebugSubscribe(nameof(PaletteAdded)),
            PaletteSelected.DebugSubscribe(nameof(PaletteSelected)),
            ProjectAdded.DebugSubscribe(nameof(ProjectAdded)),
            ProjectChanged.DebugSubscribe(nameof(ProjectChanged)),
            ProjectModified.DebugSubscribe(nameof(ProjectModified)),
            ProjectRemoved.DebugSubscribe(nameof(ProjectRemoved)),
            FrameAdded.DebugSubscribe(nameof(FrameAdded)),
            FrameModified.DebugSubscribe(nameof(FrameModified)),
            FrameChanged.DebugSubscribe(nameof(FrameChanged)),
            FrameRemoved.DebugSubscribe(nameof(FrameRemoved)),
            LayerAdded.DebugSubscribe(nameof(LayerAdded)),
            LayerModified.DebugSubscribe(nameof(LayerModified)),
            LayerRemoved.DebugSubscribe(nameof(LayerRemoved)),
            LayerChanged.DebugSubscribe(nameof(LayerChanged))
#endif
        ];

        return disposables;
    }

    public static IDisposable DebugSubscribe<T>(this Subject<T> subject, string name)
    {
        return subject.Subscribe(_ => Console.WriteLine(name));
    }
}
