using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Pixed.Android.InAppUpdates;
using Pixed.Application;
using Pixed.Application.DependencyInjection;
using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Application.Utils;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Testing;

namespace Pixed.Android;

#if DEBUG
[Activity(
    Label = "Pixed - Dev",
    Icon = "@mipmap/pixed_icon_dev",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
#else
[Activity(
    Label = "Pixed - Pixelart Editor",
    Icon = "@mipmap/pixed_icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
#endif
public class MainActivity : AvaloniaMainActivity<App>
{
    private const int RequestCode = 4711;
    private IAppUpdateManager? _updateManager;
    private AppUpdateSuccessListener? _updateSuccessListener;
    private ResumeSuccessListener? _resumeSuccessListener;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);
        PlatformSettings.MainActivity = this;
        PlatformFolder.Context = this;
        StreamBase.StreamReadImpl = typeof(AndroidStreamRead);
        StreamBase.StreamWriteImpl = typeof(AndroidStreamWrite);
        PlatformDependencyRegister.Implementation = new AndroidDependencyRegister();
        IPlatformSettings.Instance = new PlatformSettings();
        OnBackPressedDispatcher.AddCallback(new BackPressedCallback());

        base.OnCreate(savedInstanceState);

#if DEBUG
        _updateManager = new FakeAppUpdateManager(this);
#else
        _updateManager = AppUpdateManagerFactory.Create(this);
#endif
        _updateSuccessListener ??= new AppUpdateSuccessListener(
            updateManager: _updateManager,
            activity: this,
            updateRequest: RequestCode);
        _updateManager.GetAppUpdateInfo().AddOnSuccessListener(_updateSuccessListener);
        Console.WriteLine("MainActivity: Initialize AppUpdateManager");
    }

    protected override void OnResume()
    {
        base.OnResume();

        if (_updateManager is null)
        {
            Console.WriteLine("MainActivity: Update manager is null on resume");
            return;
        }

        _resumeSuccessListener ??= new ResumeSuccessListener(
            updateManager: _updateManager,
            activity: this,
            updateRequest: RequestCode);
        _updateManager.GetAppUpdateInfo().AddOnSuccessListener(_resumeSuccessListener);
    }

    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode != RequestCode)
        {
            Console.WriteLine("MainActivity: Unknown activity result request code: " + requestCode);
            return;
        }

        switch (resultCode)
        {
            case Result.Ok:
                Console.WriteLine("MainActivity: App updated");
                Toast.MakeText(this, "App updated", ToastLength.Short)?.Show();
                break;

            case Result.Canceled:
                Console.WriteLine("MainActivity: In app update cancelled");
                Toast.MakeText(this, "In app update cancelled", ToastLength.Short)?.Show();
                break;

            case (Result)Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model.ActivityResult.ResultInAppUpdateFailed:
                Console.WriteLine("MainActivity: In app update failed");
                Toast.MakeText(this, "In app update failed", ToastLength.Short)?.Show();
                break;
        }
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder).GetDefault();
    }
}

