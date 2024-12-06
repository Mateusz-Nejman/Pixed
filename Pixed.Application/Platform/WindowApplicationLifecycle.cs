using Avalonia.Controls.ApplicationLifetimes;

namespace Pixed.Application.Platform;
public class WindowApplicationLifecycle : IApplicationLifecycle
{
    public IApplicationLifetime ApplicationLifetime { get; set; }

    public bool ExtensionsEnabled => true;

    public void Close()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}
