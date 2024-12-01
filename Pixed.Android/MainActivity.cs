using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaloniaInside.Shell;
using Pixed.Application;
using Pixed.Application.Utils;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Pixed.Android;

[Activity(
    Label = "Pixed.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        PlatformFolder.Context = this;
        base.OnCreate(savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        var permissions = await CheckPermissions();

        if (permissions != PermissionStatus.Granted)
        {
            FinishAffinity();
        }
    }
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .LogToTrace()
            .UseReactiveUI()
            .UseShell()
            .SetPlatformFolder(new PlatformFolder());
    }

    private async Task<PermissionStatus> CheckPermissions()
    {
        var storageRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        var storageWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

        if (storageRead == PermissionStatus.Granted && storageWrite == PermissionStatus.Granted)
        {
            return PermissionStatus.Granted;
        }

        storageRead = await Permissions.RequestAsync<Permissions.StorageRead>();
        storageWrite = await Permissions.RequestAsync<Permissions.StorageWrite>();

        if(storageRead != PermissionStatus.Granted || storageWrite != PermissionStatus.Granted)
        {
            return PermissionStatus.Denied;
        }

        return PermissionStatus.Granted;
    }
}

