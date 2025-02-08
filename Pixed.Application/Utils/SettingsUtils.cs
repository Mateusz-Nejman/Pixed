using Newtonsoft.Json;
using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Application.Utils;
internal static class SettingsUtils
{
    private static readonly object _lock = new();
    public static async Task Save(IPlatformFolder platformFolder, ApplicationData applicationData)
    {
        var file = await platformFolder.GetFile("settings.json", FolderType.Root);
        var stream = await file.OpenWrite();
        lock (_lock)
        {
            stream.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(applicationData.UserSettings)));
            stream.Dispose();
        }
    }

    public static async Task<Settings> Load(IPlatformFolder platformFolder)
    {
        var file = await platformFolder.GetFile("settings.json", FolderType.Root);

        if (!file.Exists)
        {
            return new();
        }

        Stream stream = await file.OpenRead();
        string json = stream.ReadAllText();
        stream.Dispose();

        try
        {
            Settings settings = new();

            if (OperatingSystem.IsAndroid())
            {
                settings.PropertiesViewVisible = false;
                settings.FramesViewVisible = false;
            }

            JsonConvert.PopulateObject(json, settings);
            return settings;
        }
        catch (Exception)
        {
            return new Settings();
        }
    }
}