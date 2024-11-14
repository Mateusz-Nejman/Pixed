using Pixed.Common.Models;
using Pixed.Common.Services.Palette.Readers;
using Pixed.Common.Services.Palette.Writers;
using Pixed.Core.Models;
using System;
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

    public void Rename(PaletteModel model, string newName)
    {
        int index = Palettes.IndexOf(model);
        Palettes[index].Name = newName;
        Save(Palettes[index], true);
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

    public void Load(string filename)
    {
        FileInfo fileInfo = new(filename);

        string paletteFileName = Path.Combine(_applicationData.DataFolder.Path.AbsolutePath, "Palettes", fileInfo.Name);
        File.Copy(filename, paletteFileName, true);

        AbstractPaletteReader reader;

        if (fileInfo.Extension == ".json")
        {
            reader = new BasePaletteReader(filename);
        }
        else
        {
            reader = new GplPaletteReader(filename);
        }

        PaletteModel model = reader.Read();
        Palettes[1] = model.ToCurrentPalette();

        if (Palettes.FirstOrDefault(p => p.Id == model.Id, null) == null)
        {
            Palettes.Add(model);
        }
        else
        {
            Palettes[Palettes.FindIndex(p => p.Id == model.Id)] = model;
        }
        Subjects.PaletteSelected.OnNext(Palettes[1]);
        Subjects.PaletteAdded.OnNext(Palettes[1]);
    }

    public void Save(string filename)
    {
        var model = Palettes[_paletteIndex].ToCurrentPalette();
        model.Path = filename;
        Save(model, true);
        Save(model, false);
    }

    public void LoadAll()
    {
        var files = Directory.GetFiles(Path.Combine(_applicationData.DataFolder.Path.AbsolutePath, "Palettes"));

        foreach (var file in files)
        {
            try
            {
                FileInfo fileInfo = new(file);

                AbstractPaletteReader reader;
                if (fileInfo.Extension == ".json")
                {
                    reader = new BasePaletteReader(file);
                }
                else
                {
                    reader = new GplPaletteReader(file);
                }
                PaletteModel palette = reader.Read();
                Palettes.Add(palette);
            }
            catch (Exception e)
            {
                //ignore
            }
        }
    }

    private void Save(PaletteModel model, bool appData = false)
    {
        FileInfo fileInfo = new(model.Path);

        IAbstractPaletteWriter writer;

        if (fileInfo.Extension == ".json")
        {
            writer = new BasePaletteWriter();
        }
        else
        {
            writer = new GplPaletteWriter();
        }

        if (appData)
        {
            writer.Write(model, Path.Combine(_applicationData.DataFolder.Path.AbsolutePath, "Palettes", fileInfo.Name));
        }
        else
        {
            writer.Write(model, model.Path);
        }
    }
}
