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
    }

    protected override void OnResume()
    {
        base.OnResume();

        if (_updateManager is null)
        {
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
            return;
        }

        switch (resultCode)
        {
            case Result.Ok:
                Toast.MakeText(this, "App updated", ToastLength.Short)?.Show();
                break;

            case Result.Canceled:
                Toast.MakeText(this, "In app update cancelled", ToastLength.Short)?.Show();
                break;

            case (Result)Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model.ActivityResult.ResultInAppUpdateFailed:
                Toast.MakeText(this, "In app update failed", ToastLength.Short)?.Show();
                break;
        }
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder).GetDefault();
    }
}

