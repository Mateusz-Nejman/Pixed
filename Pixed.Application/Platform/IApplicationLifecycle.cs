using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Pixed.Application.Platform;
public interface IApplicationLifecycle
{
    public IApplicationLifetime ApplicationLifetime { get; set; }
    public bool ExtensionsEnabled { get; }
    public bool RecentFilesEnabled { get; }
    public bool ExtensionsOnSave { get; }
    public bool SingleExtensionSaveDialog { get; }
    public void Close();
    public string GetVersion();
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