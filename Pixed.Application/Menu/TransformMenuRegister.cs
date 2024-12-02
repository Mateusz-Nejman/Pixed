using Avalonia.Controls;
using Pixed.Common.Menu;
using Pixed.Common.Selection;
using Pixed.Common.Tools.Transform;
using Pixed.Core;
using Pixed.Core.Models;
using ReactiveUI;
using System;
using System.Threading.Tasks;

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
        AddToMenu(ref transformMenu, "Flip", () => RouterControl.Navigate("/transformFlip"));
        AddToMenu(ref transformMenu, "Rotation", () => RouterControl.Navigate("/transformRotate"));
        AddToMenu(ref transformMenu, "Align image to the center", () => RouterControl.Navigate("/transformAlign"));
        AddToMenu(ref transformMenu, "Crop to fit the content or the selection", new Crop(_applicationData, _selectionManager));

        _menuItemRegistry.Register(BaseMenuItem.Tools, transformMenu);
    }

    private static void AddToMenu(ref NativeMenuItem menuItem, string text, Func<Task> task)
    {
        menuItem.Menu ??= [];
        NativeMenuItem toolMenu = new(text)
        {
            Command = ReactiveCommand.CreateFromTask(task)
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

    private static void ToolAction(AbstractTransformTool tool)
    {
        tool.ApplyTransformation(false, false, false);
    }
}
