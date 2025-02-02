using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia.Controls.ApplicationLifetimes;
using Pixed.Application.Platform;
namespace Pixed.Android;
internal class PlatformSettings : IPlatformSettings
{
    public static Activity MainActivity { get; internal set; }
    public IApplicationLifetime ApplicationLifetime { get; set; }
    public bool ExtensionsEnabled => false;
    public bool RecentFilesEnabled => false;
    public bool ExtensionsOnSave => true;

    public void Close()
    {
        Process.KillProcess(Process.MyPid());
    }

    public string GetVersion()
    {
        PackageInfo info = MainActivity.PackageManager.GetPackageInfo(MainActivity.PackageName, 0);
        return info.VersionName;
    }
}
