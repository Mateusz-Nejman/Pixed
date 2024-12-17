namespace Pixed.Application.Models;
internal readonly struct PromptModel(string title, string text, string defaultValue)
{
    public string Text { get; } = text;
    public string Title { get; } = title;
    public string DefaultValue { get; } = defaultValue;
}
