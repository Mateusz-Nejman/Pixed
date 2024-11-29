using Avalonia.Controls;
using Pixed.Application.Models;
using Pixed.Application.Utils;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Core;
using Pixed.Core.Models;

namespace Pixed.Application.Menu;
internal class ViewMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle fullscreen", () =>
        {
            if (MainWindow.Handle.WindowState == WindowState.FullScreen)
            {
                MainWindow.Handle.WindowState = WindowState.Maximized;
            }
            else
            {
                MainWindow.Handle.WindowState = WindowState.FullScreen;
            }
        });

        _menuItemRegistry.Register(BaseMenuItem.View, "Grid settings", new AsyncCommand(async () =>
        {
            var navigateResult = await RouterControl.Navigate<GridSettingsResult>("/gridSettings");

            if(navigateResult.HasValue)
            {
                _applicationData.UserSettings.GridWidth = navigateResult.Value.Width;
                _applicationData.UserSettings.GridHeight = navigateResult.Value.Height;
                _applicationData.UserSettings.GridColor = navigateResult.Value.Color;
                _applicationData.UserSettings.GridEnabled = true;
                _applicationData.UserSettings.Save(_applicationData.DataFolder);
                Subjects.GridChanged.OnNext(true);
            }
        }));

        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle grid", () =>
        {
            _applicationData.UserSettings.GridEnabled = !_applicationData.UserSettings.GridEnabled;
            _applicationData.UserSettings.Save(_applicationData.DataFolder);
            Subjects.GridChanged.OnNext(true);
        });
    }
}
