using Android.Gms.Tasks;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;
using Activity = Android.App.Activity;

namespace Pixed.Android.InAppUpdates;

public class ResumeSuccessListener(
    IAppUpdateManager updateManager,
    Activity activity,
    int updateRequest)
    : Java.Lang.Object, IOnSuccessListener
{
    private readonly IAppUpdateManager _updateManager = updateManager;
    private readonly Activity _activity = activity;
    private readonly int _updateRequest = updateRequest;
    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is not AppUpdateInfo info)
        {
            return;
        }

        if (info.InstallStatus() == InstallStatus.Downloaded)
        {
            Snackbar.Show(
            _activity,
            text: "An update has just been downloaded.",
            actionText: "RESTART",
            clickHandler: _ => _updateManager.CompleteUpdate());
        }
        else if (info.UpdateAvailability() == UpdateAvailability.DeveloperTriggeredUpdateInProgress) {
            _ = _updateManager.StartUpdateFlowForResult(
                info,
                _activity,
                AppUpdateOptions
                    .NewBuilder(AppUpdateType.Immediate)
                    .Build(),
                _updateRequest);
        }
    }
}