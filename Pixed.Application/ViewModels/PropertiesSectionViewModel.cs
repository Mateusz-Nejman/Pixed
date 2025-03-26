using Pixed.Application.Controls;
using Pixed.Application.Platform;
using Pixed.Application.Utils;
using Pixed.Core.Models;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Pixed.Application.ViewModels;
internal class PropertiesSectionViewModel : ExtendedViewModel
{
    private readonly ApplicationData _applicationData;
    private readonly IPlatformFolder _platformFolder;
    private bool _isVisible = true;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            OnPropertyChanged();
            _applicationData.UserSettings.PropertiesViewVisible = value;
            IsVisibleChanged.OnNext(value);
            Task.Run(async () =>
            {
                await SettingsUtils.Save(_platformFolder, _applicationData);
            });
        }
    }

    public Subject<bool> IsVisibleChanged { get; } = new Subject<bool>();

    public PropertiesSectionViewModel(ApplicationData applicationData, IPlatformFolder platformFolder)
    {
        _applicationData = applicationData;
        _platformFolder = platformFolder;
        _isVisible = _applicationData.UserSettings.PropertiesViewVisible;
        OnPropertyChanged(nameof(IsVisible));
    }
}
