using Avalonia.Controls.ApplicationLifetimes;

namespace Pixed.Application.Platform;
public interface IPlatformSettings
{
    public static IPlatformSettings Instance { get; set; } = new DefaultPlatformSettings();
    public IApplicationLifetime ApplicationLifetime { get; set; }
    public bool ExtensionsEnabled { get; }
    public bool RecentFilesEnabled { get; }
    public bool ExtensionsOnSave { get; }
    public void Close();
    public string GetVersion();
}