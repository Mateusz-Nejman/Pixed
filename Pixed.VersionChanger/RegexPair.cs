namespace Pixed.VersionChanger;

public readonly struct RegexPair(string file, string pattern)
{
    public string File { get; } = file;
    public string Pattern { get; } = pattern;
}