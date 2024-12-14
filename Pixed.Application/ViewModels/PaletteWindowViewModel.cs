using Pixed.Application.Controls;
using Pixed.Application.Routing;
using Pixed.Common.Models;
using Pixed.Common.Services.Palette;
using Pixed.Core;
using Pixed.Core.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class PaletteWindowViewModel : PixedViewModel
{
    public struct PaletteData
    {
        public PixedImage BitmapImage { get; set; }
        public int BitmapWidth { get; set; }
        public int BitmapHeight { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public PaletteModel Model { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RenameCommand { get; set; }
    }

    private ObservableCollection<PaletteData> _palettes;
    private readonly PaletteService _paletteService;

    public ObservableCollection<PaletteData> Palettes
    {
        get => _palettes;
        set
        {
            _palettes = value;
            OnPropertyChanged();
        }
    }

    public Action<bool, PaletteModel> PaletteAction { get; set; }
    public Action<PaletteModel, string> PaletteRenameAction { get; set; }

    public PaletteWindowViewModel(PaletteService paletteService)
    {
        _paletteService = paletteService;
        _palettes = [];
        Initialize();
    }

    private void Initialize()
    {
        Palettes.Clear();

        for (int a = 2; a < _paletteService.Palettes.Count; a++)
        {
            PaletteModel model = _paletteService.Palettes[a];

            if (model.Colors.Count == 0)
            {
                continue;
            }

            var paletteBitmap = GeneratePaleteBitmap(model);
            Palettes.Add(new PaletteData
            {
                Name = model.Name,
                Path = model.Path,
                Model = model,
                BitmapImage = new PixedImage(paletteBitmap),
                BitmapWidth = paletteBitmap.Width,
                BitmapHeight = paletteBitmap.Height,
                SelectCommand = new ActionCommand<PaletteModel>(m => PaletteAction?.Invoke(true, m)),
                RemoveCommand = new ActionCommand<PaletteModel>(m => PaletteAction?.Invoke(false, m)),
                RenameCommand = new ActionCommand<PaletteModel>(async (m) =>
                {
                    var result = await Router.Prompt("Rename Palette", "New name: ", m.Name);
                    //TODO Waiting for AvaloniaInside.Shell author fix https://github.com/AvaloniaInside/Shell/issues/64

                    if (result.HasValue)
                    {
                        PaletteRenameAction?.Invoke(m, result.Value);
                        Initialize();
                    }
                })
            });
        }
    }

    private static SKBitmap GeneratePaleteBitmap(PaletteModel model)
    {
        int mod = (model.Colors.Count % 10) == 0 ? 0 : 1;
        SKBitmap bitmap = new(200, ((model.Colors.Count / 10) + mod) * 20);
        SKCanvas canvas = new(bitmap);
        int x = 0;
        int y = 0;

        foreach (var color in model.Colors)
        {
            int rectX = x * 20;
            int rectY = y * 20;

            canvas.DrawRect(SKRect.Create(rectX, rectY, 20, 20), new SKPaint() { Color = (UniColor)color });
            x++;

            if (x == 10)
            {
                x = 0;
                y++;
            }
        }
        canvas.Dispose();
        return bitmap;
    }
}
