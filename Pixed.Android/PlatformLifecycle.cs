using Android.App;
using Avalonia.Controls.ApplicationLifetimes;
using Pixed.Application.Platform;
namespace Pixed.Android;
internal class PlatformLifecycle : IApplicationLifecycle
{
    public static Activity MainActivity { get; internal set; }
    public IApplicationLifetime ApplicationLifetime { get; set; }
    public bool ExtensionsEnabled => false;
    public bool RecentFilesEnabled => false;

    public void Close()
    {
        MainActivity?.FinishAffinity();
    }

    public string GetVersion()
    {
        return Xamarin.Essentials.VersionTracking.CurrentVersion;
    }
}
