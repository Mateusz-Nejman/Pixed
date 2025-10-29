using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install;
using System;
using Android.App;
using Android.Widget;
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
                case InstallStatus.Pending:
                case InstallStatus.Installing:
                case InstallStatus.Installed:
                case InstallStatus.Canceled:
                    break;

                case InstallStatus.Downloaded:
                    Snackbar.Show(
                    _activity,
                    text: "An update has just been downloaded.",
                    actionText: "RESTART",
                    clickHandler: _ => _updateManager?.CompleteUpdate());
                    break;
                
                case InstallStatus.Failed:
                    Toast.MakeText(_activity, "Update download failed.", ToastLength.Short)?.Show();
                    break;
            }
        }
        catch (Exception)
        {
            //Ignore
        }
    }
}
