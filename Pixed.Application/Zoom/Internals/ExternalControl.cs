using Avalonia.Controls;

namespace Pixed.Application.Zoom.Internals;

internal class ExternalControl : Border
{
    private bool _enabled = true;
    //TODO new zoom gesture implementation

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
        }
    }
}