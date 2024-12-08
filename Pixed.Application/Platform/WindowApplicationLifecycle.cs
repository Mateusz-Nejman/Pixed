using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;

namespace Pixed.Application.Platform;
public class WindowApplicationLifecycle : IApplicationLifecycle
{
    public IApplicationLifetime ApplicationLifetime { get; set; }

    public bool ExtensionsEnabled => true;
    public bool RecentFilesEnabled => true;
    public bool ExtensionsOnSave => false;

    public void Close()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }

    public string GetVersion()
    {
        var assemblyName = Process.GetCurrentProcess().MainModule.FileName;
        var versionInfo = FileVersionInfo.GetVersionInfo(assemblyName);
        return versionInfo.FileVersion;
    }
}
