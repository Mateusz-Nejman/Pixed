using Microsoft.Extensions.Configuration;
using Pixed.Application.IO;
using Pixed.Core.Models;

Console.WriteLine("Pixed Exporter by Mateusz Nejman");
Console.WriteLine("Usage:");
Console.WriteLine("--input [dir]: Directory where exporter should find Pixed projects");
Console.WriteLine("--output [dir]: Directory where exporter should export PNG files");

IConfiguration config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

string inputFolder = config["input"] ?? string.Empty;
string outputFolder = config["output"] ?? string.Empty;

if (inputFolder == string.Empty || outputFolder == string.Empty)
{
    Console.Error.WriteLine("Invalid arguments");
    return;
}

string[] files = Directory.GetFiles(inputFolder);

PngProjectSerializer pngSerializer = new();
PixedProjectSerializer pixedSerializer = new();
ApplicationData applicationData = new()
{
    UserSettings = new Settings() { UserWidth = 16, UserHeight = 16 }
};

foreach (string file in files)
{
    FileInfo info = new(file);
    Console.WriteLine(info.Name);
    if (info.Extension != ".pixed")
    {
        continue;
    }

    var stream = info.OpenRead();
    var model = pixedSerializer.Deserialize(stream, applicationData);
    stream.Dispose();

    string name = Path.Combine(outputFolder, info.Name.Replace(".pixed", ".png"));
    var pngStream = File.OpenWrite(name);
    pngSerializer.Serialize(pngStream, model, true);
}