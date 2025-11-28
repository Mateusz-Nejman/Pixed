using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install;
using Xamarin.Google.Android.Play.Core.AppUpdate;

namespace Pixed.Android.InAppUpdates;

public class InstallStateUpdatedListener(Activity activity, IAppUpdateManager updateManager)
        : Java.Lang.Object, IInstallStateUpdatedListener
{
    private readonly Activity _activity = activity;
    private readonly IAppUpdateManager _updateManager = updateManager;

    public void OnStateUpdate(InstallState? state)
    {
        state = state ?? throw new ArgumentNullException(nameof(state));
        
        try
        {
            var installStatus = state.InstallStatus();
            switch (installStatus)
            {
                case InstallStatus.Unknown:
                    Console.WriteLine($"AppUpdateSuccessListener: InstallStatus: Unknown");
                    break;
                case InstallStatus.Pending:
                    Console.WriteLine($"AppUpdateSuccessListener: InstallStatus: Pending");
                    break;
                case InstallStatus.Installing:
                    Console.WriteLine($"AppUpdateSuccessListener: InstallStatus: Installing");
                    break;
                case InstallStatus.Installed:
                    Console.WriteLine($"AppUpdateSuccessListener: InstallStatus: Installed");
                    break;
                case InstallStatus.Canceled:
                    Console.WriteLine($"AppUpdateSuccessListener: InstallStatus: Cancelled");
                    break;

                case InstallStatus.Downloaded:
                    Console.WriteLine("AppUpdateSuccessListener: An update has just been downloaded.");
                    Snackbar.Show(
                    _activity,
                    text: "An update has just been downloaded.",
                    actionText: "RESTART",
                    clickHandler: _ => _updateManager.CompleteUpdate());
                    break;
                
                case InstallStatus.Failed:
                    Console.WriteLine("AppUpdateSuccessListener: Update download failed.");
                    Toast.MakeText(_activity, "Update download failed.", ToastLength.Short)?.Show();
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("AppUpdateSuccessListener: " + e.ToString());
        }
    }
}
