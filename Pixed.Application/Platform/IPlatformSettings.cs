using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Pixed.Application.Platform;
public interface IPlatformSettings
{
    public IApplicationLifetime ApplicationLifetime { get; set; }
    public bool ExtensionsEnabled { get; }
    public bool RecentFilesEnabled { get; }
    public bool ExtensionsOnSave { get; }
    public bool SingleExtensionSaveDialog { get; }
    public void Close();
    public string GetVersion();
}

public static class PlatformSettings
{
    public static IPlatformSettings Lifecycle { get; set; } = new DefaultPlatformSettings();
    public static AppBuilder SetSettings(this AppBuilder app, IPlatformSettings lifecycle)
    {
        Lifecycle = lifecycle;
        return app;
    }
}