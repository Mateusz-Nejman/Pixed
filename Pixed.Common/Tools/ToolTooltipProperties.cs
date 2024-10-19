namespace Pixed.Common.Tools;
public readonly struct ToolTooltipProperties(string title, string? button1, string? button1text, string? button2, string? button2text, string? button3, string? button3text)
{
    public string Title { get; } = title;
    public string? Button1 { get; } = button1;
    public string? Button1Text { get; } = button1text;
    public string? Button2 { get; } = button2;
    public string? Button2Text { get; } = button2text;
    public string? Button3 { get; } = button3;
    public string? Button3Text { get; } = button3text;

    public ToolTooltipProperties(string title) : this(title, null, null)
    {
    }

    public ToolTooltipProperties(string title, string? button1, string? button1text) : this(title, button1, button1text, null, null)
    {
    }

    public ToolTooltipProperties(string title, string? button1, string? button1text, string? button2, string? button2text) : this(title, button1, button1text, button2, button2text, null, null)
    {
    }

    public bool HasButtons()
    {
        return Button1 != null || Button1Text != null || Button2 != null || Button2Text != null || Button3 != null || Button3Text != null;
    }
}