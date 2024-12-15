using Pixed.Common.Models;
using Pixed.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pixed.Common.Services.Palette;

public class PaletteService
{
    private readonly ApplicationData _applicationData;
    public List<PaletteModel> Palettes { get; }
    public PaletteModel CurrentColorsPalette => Palettes[0];
    public PaletteModel SelectedPalette => Palettes[_paletteIndex];
    private int _paletteIndex => 1;

    public PaletteService(ApplicationData applicationData)
    {
        _applicationData = applicationData;
        Palettes = [];
        Palettes.Add(new PaletteModel("default") { Name = "Current colors" });
        Palettes.Add(new PaletteModel("palette") { Name = "Palette" });
    }

    public void Select(PaletteModel model)
    {
        int index = Palettes.IndexOf(model);
        Palettes[1] = Palettes[index].ToCurrentPalette();
        Subjects.PaletteSelected.OnNext(model);
    }

    public void Remove(PaletteModel model)
    {
        int index = Palettes.IndexOf(model);
        Palettes.RemoveAt(index);

        if (File.Exists(model.Path))
        {
            File.Delete(model.Path);
        }
        Subjects.PaletteSelected.OnNext(Palettes[1]);
    }

    public void SetCurrentColors()
    {
        Palettes[0].Colors = _applicationData.CurrentModel.GetAllColors();
    }

    public void ReplaceSecondPalette(PaletteModel palette)
    {
        Palettes[_paletteIndex] = palette;
    }

    public void AddColorsFromPalette(PaletteModel palette)
    {
        SelectedPalette.Colors.AddRange(palette.Colors.Where(c => !SelectedPalette.Colors.Contains(c)));
        SelectedPalette.Colors.Sort();
    }

    public void AddPrimaryColor()
    {
        if (!SelectedPalette.Colors.Contains(_applicationData.PrimaryColor))
        {
            SelectedPalette.Colors.Add(_applicationData.PrimaryColor);
            SelectedPalette.Colors.Sort();
        }
    }

    public void ClearPalette()
    {
        SelectedPalette.Colors.Clear();
        Subjects.PaletteSelected.OnNext(Palettes[1]);
    }

    public void Add(PaletteModel palette)
    {
        if (Palettes.FirstOrDefault(p => p.Id == palette.Id, null) == null)
        {
            Palettes.Add(palette);
            Subjects.PaletteAdded.OnNext(palette);
        }
    }
}
