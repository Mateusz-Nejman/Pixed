// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

string lineRegex = @"^(\s*\d{1,3})(\s*\d{1,3})(\s*\d{1,3})";
string nameRegex = @"name\s*\:\s*(.*)$";

var lines = File.ReadAllLines("pico-8.gpl");
string name = string.Empty;

foreach(var line in lines)
{
    if(name == string.Empty)
    {
        Match nameMatch = Regex.Match(line, nameRegex, RegexOptions.IgnoreCase);

        if(nameMatch.Success)
        {
            name = nameMatch.Groups[1].Value;
            continue;
        }
    }
    
    Match lineMatch = Regex.Match(line, lineRegex);

    if(lineMatch.Success)
    {
        int r = int.Parse(lineMatch.Groups[1].Value);
        int g = int.Parse(lineMatch.Groups[2].Value);
        int b = int.Parse(lineMatch.Groups[3].Value);
    }
}