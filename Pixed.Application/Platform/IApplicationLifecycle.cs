using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Pixed.Application.Platform;
public interface IApplicationLifecycle
{
    public IApplicationLifetime ApplicationLifetime { get; set; }
    public bool ExtensionsEnabled { get; }
    public void Close();
}

public static class PlatformLifecycle
{
    public static IApplicationLifecycle Lifecycle { get; set; } = new WindowApplicationLifecycle();
    public static AppBuilder SetLifecycle(this AppBuilder app, IApplicationLifecycle lifecycle)
    {
        Lifecycle = lifecycle;
        return app;
    }
}