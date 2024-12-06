using Avalonia.Controls;
using Pixed.Application.Routing;
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
        MenuItem transformMenu = new("Transform")
        {
            Items = []
        };
        AddToMenu(ref transformMenu, "Flip", () => Router.Navigate("/transformFlip"));
        AddToMenu(ref transformMenu, "Rotation", () => Router.Navigate("/transformRotate"));
        AddToMenu(ref transformMenu, "Align image to the center", () => Router.Navigate("/transformAlign"));
        AddToMenu(ref transformMenu, "Crop to fit the content or the selection", new Crop(_applicationData, _selectionManager));

        _menuItemRegistry.Register(BaseMenuItem.Tools, transformMenu);
    }

    private static void AddToMenu(ref MenuItem menuItem, string text, Func<Task> task)
    {
        menuItem.Items ??= [];
        MenuItem toolMenu = new(text)
        {
            Command = ReactiveCommand.CreateFromTask(task)
        };

        menuItem.Items.Add(toolMenu);
    }

    private static void AddToMenu(ref MenuItem menuItem, string text, AbstractTransformTool tool)
    {
        menuItem.Items ??= [];
        MenuItem toolMenu = new(text)
        {
            Command = new ActionCommand<AbstractTransformTool>(ToolAction),
            CommandParameter = tool
        };

        menuItem.Items.Add(toolMenu);
    }

    private static void ToolAction(AbstractTransformTool tool)
    {
        tool.ApplyTransformation(false, false, false);
    }
}
