using Avalonia.Controls;
using Pixed.Models;
using Pixed.Windows;
using static Pixed.Menu.MenuBuilder;

namespace Pixed.Menu;
internal class ViewMenuRegister(MenuItemRegistry menuItemRegistry, ApplicationData applicationData)
{
    private readonly MenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle fullscreen", () =>
        {
            if(MainWindow.Handle.WindowState == WindowState.FullScreen)
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
            GridSettingsWindow window = new(_applicationData);
            var success = await window.ShowDialog<bool>(MainWindow.Handle);
            if (success)
            {
                _applicationData.UserSettings.GridWidth = window.WidthValue;
                _applicationData.UserSettings.GridHeight = window.HeightValue;
                _applicationData.UserSettings.GridColor = window.GridColor;
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
