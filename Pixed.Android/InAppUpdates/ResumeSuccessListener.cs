using Android.Gms.Tasks;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;

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
            Console.WriteLine("ResumeSuccessListener: OnSuccess received a result of unexpected type.");
            return;
        }

        if (info.InstallStatus() == InstallStatus.Downloaded)
        {
            Console.WriteLine("ResumeSuccessListener: An update has been downloaded - prompting user to restart.");
            Snackbar.Show(
            _activity,
            text: "An update has just been downloaded.",
            actionText: "RESTART",
            clickHandler: _ => _updateManager.CompleteUpdate());
        }
        else if (info.UpdateAvailability() == UpdateAvailability.DeveloperTriggeredUpdateInProgress) {
            Console.WriteLine("ResumeSuccessListener: DeveloperTriggeredUpdateInProgress.");
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