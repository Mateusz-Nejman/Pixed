﻿using Pixed.Common.Models;
using System.IO;
using System.Text.RegularExpressions;

namespace Pixed.Common.Services.Palette.Readers;

internal class GplPaletteReader(string filename) : AbstractPaletteReader(filename)
{
    public override PaletteModel Read()
    {
        string lineRegex = @"^(\s*\d{1,3})(\s*\d{1,3})(\s*\d{1,3})";
        string nameRegex = @"name\s*\:\s*(.*)$";

        FileInfo info = new(_filename);
        PaletteModel model = new(info.Name)
        {
            Path = _filename
        };

        var lines = File.ReadAllLines(_filename);

        foreach (var line in lines)
        {
            if (model.Name == string.Empty)
            {
                Match nameMatch = Regex.Match(line, nameRegex, RegexOptions.IgnoreCase);

                if (nameMatch.Success)
                {
                    model.Name = nameMatch.Groups[1].Value;
                    continue;
                }
            }

            Match lineMatch = Regex.Match(line, lineRegex);

            if (lineMatch.Success)
            {
                int r = int.Parse(lineMatch.Groups[1].Value);
                int g = int.Parse(lineMatch.Groups[2].Value);
                int b = int.Parse(lineMatch.Groups[3].Value);
                model.Colors.Add(new UniColor(r, g, b));
            }
        }

        if (model.Name == string.Empty)
        {
            model.Name = info.Name;
        }

        return model;
    }
}
