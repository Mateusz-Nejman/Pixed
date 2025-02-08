using Pixed.Application.Controls;
using Pixed.Application.Platform;
using Pixed.Application.Utils;
using Pixed.Common.Menu;
using Pixed.Core;
using Pixed.Core.Models;
using System.Threading.Tasks;
using System.Windows.Input;

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
            Task.Run(async () =>
            {
                await SettingsUtils.Save(_platformFolder, _applicationData);
            });
        }
    }

    public ICommand CloseViewCommand { get; }

    public PropertiesSectionViewModel(ApplicationData applicationData, IPlatformFolder platformFolder, IMenuItemRegistry menuItemRegistry)
    {
        _applicationData = applicationData;
        _platformFolder = platformFolder;
        CloseViewCommand = new ActionCommand(CloseView);
        _isVisible = _applicationData.UserSettings.PropertiesViewVisible;
        OnPropertyChanged(nameof(IsVisible));

        menuItemRegistry.Register(BaseMenuItem.Base, "Toggle properties view", ToggleView, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_color_24_regular.svg"));
    }

    private void CloseView()
    {
        IsVisible = false;
    }

    private void ToggleView()
    {
        IsVisible = !IsVisible;
    }
}
