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
    public static async Task Save(IPlatformFolder platformFolder, ApplicationData applicationData)
    {
        var file = await platformFolder.GetFile("settings.json", FolderType.Root);
        var stream = await file.OpenWrite();
        stream.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(applicationData.UserSettings)));
        stream.Dispose();
    }

    public static async Task<Settings> Load(IPlatformFolder platformFolder)
    {
        Stream? stream = null;
        try
        {
            var file = await platformFolder.GetFile("settings.json", FolderType.Root);
            stream = await file.OpenRead();
            string json = stream.ReadAllText();
            stream.Dispose();
            return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
        }
        catch (Exception)
        {
            stream?.Dispose();
        }

        return new Settings();
    }
}