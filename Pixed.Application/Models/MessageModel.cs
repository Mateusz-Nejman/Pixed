namespace Pixed.Application.Models;
internal readonly struct MessageModel(string title, string text)
{
    public string Text { get; } = text;
    public string Title { get; } = title;
}
