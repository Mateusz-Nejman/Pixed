using Android.Gms.Tasks;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;

namespace Pixed.Android.InAppUpdates;

public class AppUpdateSuccessListener(
    IAppUpdateManager updateManager,
    Activity activity,
    int updateRequest)
    : Java.Lang.Object, IOnSuccessListener
{
    private readonly IAppUpdateManager _updateManager = updateManager;
    private readonly Activity _activity = activity;
    private readonly int _updateRequest = updateRequest;
    private InstallStateUpdatedListener? _installStateListener;

    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is not AppUpdateInfo info)
        {
            return;
        }

        var updateAvailability = info.UpdateAvailability();
        var updatePriority = info.UpdatePriority();
        var isImmediateUpdatesAllowed = info.IsUpdateTypeAllowed(AppUpdateType.Immediate);
        var isFlexibleUpdatesAllowed = info.IsUpdateTypeAllowed(AppUpdateType.Flexible);
        switch (updateAvailability)
        {
            case UpdateAvailability.UpdateAvailable or
                UpdateAvailability.DeveloperTriggeredUpdateInProgress
                when updatePriority >= 4 &&
                     isImmediateUpdatesAllowed:
                {
                    _ = _updateManager.StartUpdateFlowForResult(
                        info,
                        _activity,
                        AppUpdateOptions
                            .NewBuilder(AppUpdateType.Immediate)
                            .Build(),
                        _updateRequest);
                    break;
                }

            case UpdateAvailability.UpdateAvailable or
                UpdateAvailability.DeveloperTriggeredUpdateInProgress
                when isFlexibleUpdatesAllowed:
                {
                    _installStateListener ??= new InstallStateUpdatedListener(_activity, _updateManager);
                    _updateManager.RegisterListener(_installStateListener);

                    _ = _updateManager.StartUpdateFlowForResult(
                        info,
                        _activity,
                        AppUpdateOptions
                            .NewBuilder(AppUpdateType.Flexible)
                            .Build(),
                        _updateRequest);
                    break;
                }

            case UpdateAvailability.UpdateNotAvailable:
            case UpdateAvailability.Unknown:
                break;
        }
    }
}