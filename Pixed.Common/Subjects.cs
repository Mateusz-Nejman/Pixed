using Pixed.Common.Models;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Selection;
using SkiaSharp;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Pixed.Common;

public static class Subjects
{
    public static Subject<KeyState> KeyState { get; } = new Subject<KeyState>();
    public static Subject<BaseSelection> SelectionCreating { get; } = new Subject<BaseSelection>();
    public static Subject<BaseSelection> SelectionDismissed { get; } = new Subject<BaseSelection>();
    public static Subject<BaseSelection> SelectionCreated { get; } = new Subject<BaseSelection>();
    public static Subject<BaseSelection> SelectionStarted { get; } = new Subject<BaseSelection>();
    public static Subject<bool> GridChanged { get; } = new Subject<bool>();
    public static Subject<bool> AnimationPreviewChanged { get; } = new Subject<bool>();
    public static Subject<BaseToolPair> ToolChanged { get; } = new Subject<BaseToolPair>();
    public static Subject<string[]> NewInstanceHandled { get; } = new Subject<string[]>();
    public static Subject<List<Pixel>> CurrentLayerRenderModified { get; } = new Subject<List<Pixel>>();

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
}
