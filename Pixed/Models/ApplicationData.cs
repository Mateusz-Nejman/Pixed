using Avalonia.Platform.Storage;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pixed.Models;
internal class ApplicationData
{
    private int _index = 0;
    public IStorageFolder? DataFolder { get; set; }
    public int CurrentModelIndex { get; set; }
    public PixedModel CurrentModel => Models[CurrentModelIndex];
    public Frame CurrentFrame => CurrentModel.CurrentFrame;
    public Layer CurrentLayer => CurrentFrame.CurrentLayer;
    public ObservableCollection<PixedModel> Models { get; } = [];

    public UniColor PrimaryColor { get; set; }
    public UniColor SecondaryColor { get; set; }
    public Settings UserSettings { get; set; }
    public int ToolSize { get; set; } = 1;

    public ApplicationData()
    {
        
    }

    public void Initialize(IStorageFolder pixedFolder)
    {
        DataFolder = pixedFolder;
        UserSettings = Settings.Load(DataFolder);
        Models.Add(new PixedModel(this, UserSettings.UserWidth, UserSettings.UserHeight));
        CurrentModel.FileName = GenerateName();
        CurrentModel.AddHistory(false);
        CurrentModel.UnsavedChanges = false;
    }

    public string GenerateName(string prefix = "Untitled")
    {
        string name = prefix + "-" + _index;
        _index++;

        if (Models.Any(m => m.FileName == name))
        {
            return GenerateName(prefix);
        }

        return name;
    }
}
