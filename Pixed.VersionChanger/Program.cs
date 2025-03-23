using Pixed.VersionChanger;
using System.Text.RegularExpressions;

RegexPair[] pairs = [
    new RegexPair("/buildDistribution.bat", "set VERSION=(.+?)\r\n"),
    new RegexPair("/msi.wxs", "Version=\"(.+?)\""),
    new RegexPair("/Pixed.Android/Properties/AndroidManifest.xml", "android:versionName=\"(.+?)\""),
    new RegexPair("/Pixed.Android/Properties/AndroidManifest.xml", "android:versionCode=\"(.+?)\""),
    new RegexPair("/Pixed.Desktop/Pixed.Desktop.csproj", "<AssemblyVersion>(.+?)<\\/AssemblyVersion>"),
    new RegexPair("/Pixed.MSStore/Package.appxmanifest", "Version=\"(.+?)\"")
];

string currentDirectory = Environment.CurrentDirectory;
string pixedDirectory = currentDirectory.Substring(0, currentDirectory.IndexOf("Pixed") + 5);
Console.WriteLine("Pixed directory: " + pixedDirectory);

foreach (var pair in pairs)
{
    Console.WriteLine("Processing " + pair.File + " file");
    string fileContent = File.ReadAllText(pixedDirectory + pair.File);
    var match = Regex.Match(fileContent, pair.Pattern);

    if(match.Success || match.Groups.Count < 2)
    {
        var versionMatch = match.Groups[0].Value;
        var oldVersionMatch = match.Groups[1].Value;
        Console.WriteLine("Old version: " + oldVersionMatch);
        Console.WriteLine("New version: ");
        string? newVersion = Console.ReadLine();

        if(newVersion == null)
        {
            continue;
        }
        var newVersionMatch = versionMatch.Replace(oldVersionMatch, newVersion);

        Regex regex = new(pair.Pattern);
        string newContent = regex.Replace(fileContent, newVersionMatch, 1);
        File.WriteAllText(pixedDirectory + pair.File, newContent);
    }
    else
    {
        Console.WriteLine("Version not found in " + pair.File);
    }
}
Console.ReadKey();