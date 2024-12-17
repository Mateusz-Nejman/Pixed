namespace Pixed.Application.Models;
internal readonly struct NumericPromptModel(string title, string text, double defaultValue)
{
    public string Text { get; } = text;
    public string Title { get; } = title;
    public double DefaultValue { get; } = defaultValue;
}
