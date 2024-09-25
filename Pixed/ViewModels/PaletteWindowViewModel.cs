using Pixed.Controls;
using Pixed.Models;
using Pixed.Services.Palette;
using Pixed.Utils;
using Pixed.Windows;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Pixed.ViewModels;

internal class PaletteWindowViewModel : PixedViewModel
{
    public struct PaletteData
    {
        public Bitmap BitmapImage { get; set; }
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

            Palettes.Add(new PaletteData
            {
                Name = model.Name,
                Path = model.Path,
                Model = model,
                BitmapImage = GeneratePaleteBitmap(model),
                SelectCommand = new ActionCommand<PaletteModel>(m => PaletteAction?.Invoke(true, m)),
                RemoveCommand = new ActionCommand<PaletteModel>(m => PaletteAction?.Invoke(false, m)),
                RenameCommand = new ActionCommand<PaletteModel>(async (m) =>
                {
                    Prompt window = new()
                    {
                        Title = "Rename Palette",
                        Text = "New name: ",
                        DefaultValue = m.Name
                    };

                    if (await window.ShowDialog<bool>(MainWindow.Handle) == true)
                    {
                        PaletteRenameAction?.Invoke(m, window.Value);
                        Initialize();
                    }
                })
            });
        }
    }

    private static Bitmap GeneratePaleteBitmap(PaletteModel model)
    {
        int mod = (model.Colors.Count % 10) == 0 ? 0 : 1;
        System.Drawing.Bitmap bitmap = new(200, ((model.Colors.Count / 10) + mod) * 20);
        Graphics graphics = Graphics.FromImage(bitmap);
        int x = 0;
        int y = 0;

        foreach (var color in model.Colors)
        {
            int rectX = x * 20;
            int rectY = y * 20;

            graphics.FillRectangle(new SolidBrush((UniColor)color), new Rectangle(rectX, rectY, 20, 20));
            x++;

            if (x == 10)
            {
                x = 0;
                y++;
            }
        }
        graphics.Dispose();
        return bitmap.ToAvaloniaBitmap();
    }
}
