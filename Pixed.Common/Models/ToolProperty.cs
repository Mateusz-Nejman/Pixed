namespace Pixed.Common.Models;
public class ToolProperty(string name) : PropertyChangedBase
{
    private bool _checked = false;

    public bool Checked
    {
        get => _checked;
        set
        {
            _checked = value;
            OnPropertyChanged();
        }
    }

    public string Name { get; } = name;

    public ToolProperty Clone()
    {
        return new ToolProperty(Name);
    }
}
