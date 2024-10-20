using Avalonia.Controls;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Models;
using Pixed.Common.Selection;
using Pixed.Common.Tools.Transform;
using System;

namespace Pixed.Application.Menu;
internal class TransformMenuRegister(IMenuItemRegistry menuItemRegistry, SelectionManager selectionManager, ApplicationData applicationData)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly SelectionManager _selectionManager = selectionManager;
    private readonly ApplicationData _applicationData = applicationData;

    public void Register()
    {
        NativeMenuItem transformMenu = new("Transform")
        {
            Menu = []
        };
        AddToMenu(ref transformMenu, "Flip", () => new TransformFlipWindow(_applicationData));
        AddToMenu(ref transformMenu, "Rotation", () => new TransformRotateWindow(_applicationData));
        AddToMenu(ref transformMenu, "Align image to the center", () => new TransformAlignWindow(_applicationData));
        AddToMenu(ref transformMenu, "Crop to fit the content or the selection", new Crop(_applicationData, _selectionManager));

        _menuItemRegistry.Register(BaseMenuItem.Tools, transformMenu);
    }

    private static void AddToMenu(ref NativeMenuItem menuItem, string text, Func<Window> windowCreator)
    {
        menuItem.Menu ??= [];
        NativeMenuItem toolMenu = new(text)
        {
            Command = new ActionCommand<Func<Window>>(WindowToolAction),
            CommandParameter = windowCreator
        };

        menuItem.Menu.Add(toolMenu);
    }

    private static void AddToMenu(ref NativeMenuItem menuItem, string text, AbstractTransformTool tool)
    {
        menuItem.Menu ??= [];
        NativeMenuItem toolMenu = new(text)
        {
            Command = new ActionCommand<AbstractTransformTool>(ToolAction),
            CommandParameter = tool
        };

        menuItem.Menu.Add(toolMenu);
    }

    private static void WindowToolAction(Func<Window> windowCreator)
    {
        var window = windowCreator();
        window.ShowDialog(MainWindow.Handle);
    }

    private static void ToolAction(AbstractTransformTool tool)
    {
        tool.ApplyTransformation(false, false, false);
    }
}
