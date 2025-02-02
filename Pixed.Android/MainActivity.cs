using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using AvaloniaInside.Shell;
using Pixed.Application;
using Pixed.Application.DependencyInjection;
using Pixed.Application.IO;
using Pixed.Application.Platform;
using System.Threading.Tasks;

namespace Pixed.Android;

[Activity(
    Label = "Pixed - Pixelart Editor",
    Icon = "@mipmap/pixed_icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    private TaskCompletionSource<Permission> _permissionTcs;
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);
        PlatformSettings.MainActivity = this;
        PlatformFolder.Context = this;
        StreamBase.StreamReadImpl = typeof(AndroidStreamRead);
        StreamBase.StreamWriteImpl = typeof(AndroidStreamWrite);
        PlatformDependencyRegister.Implementation = new AndroidDependencyRegister();
        IPlatformSettings.Instance = new PlatformSettings();

        base.OnCreate(savedInstanceState);
        var permissions = await CheckPermissions();

        if (permissions != Permission.Granted)
        {
            Process.KillProcess(Process.MyPid());
            return;
        }
    }
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .UseShell();
    }

    private Task<Permission> CheckPermissions()
    {
        if (Environment.IsExternalStorageManager)
        {
            return Task.FromResult(Permission.Granted);
        }

        if (_permissionTcs != null && _permissionTcs.Task.IsCompleted)
        {
            _permissionTcs.SetCanceled();
            _permissionTcs = null;
        }

        Intent intent = new(global::Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
        intent.AddCategory("android.intent.category.DEFAULT");
        intent.SetData(global::Android.Net.Uri.Parse("package:" + PackageName));
        StartActivityForResult(intent, 1);
        _permissionTcs = new TaskCompletionSource<Permission>();
        return _permissionTcs.Task;
    }

    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == 1)
        {
            if (Environment.IsExternalStorageManager)
            {
                _permissionTcs.TrySetResult(Permission.Granted);
                return;
            }

            _permissionTcs.TrySetResult(Permission.Denied);
        }
    }
}

