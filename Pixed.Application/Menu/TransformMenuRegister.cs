using Pixed.Application.Routing;
using Pixed.Common.Menu;
using Pixed.Common.Services;
using Pixed.Common.Tools;
using Pixed.Common.Tools.Transform;
using Pixed.Core;
using Pixed.Core.Models;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace Pixed.Application.Menu;
internal class TransformMenuRegister(IMenuItemRegistry menuItemRegistry, SelectionManager selectionManager, ApplicationData applicationData, IHistoryService historyService)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly SelectionManager _selectionManager = selectionManager;
    private readonly ApplicationData _applicationData = applicationData;
    private readonly IHistoryService _historyService = historyService;

    public void Register()
    {
        MenuItem transformMenu = new("Transform")
        {
            Items = []
        };
        AddToMenu(ref transformMenu, "Flip", () => Router.Navigate("/transformFlip"), new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_flip_horizontal_48_regular.svg"));
        AddToMenu(ref transformMenu, "Rotation", () => Router.Navigate("/transformRotate"), new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_rotate_clockwise_24_regular.svg"));
        AddToMenu(ref transformMenu, "Align image to the center", () => Router.Navigate("/transformAlign"), new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_move_inward_20_regular.svg"));
        AddToMenu(ref transformMenu, "Crop to fit the content or the selection", new Crop(_applicationData, _selectionManager, _historyService), new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_crop_48_regular.svg"));

        _menuItemRegistry.Register(BaseMenuItem.Tools, transformMenu);
    }

    private static void AddToMenu(ref MenuItem menuItem, string text, Func<Task> task, Uri icon)
    {
        menuItem.Items ??= [];
        MenuItem toolMenu = new(text)
        {
            Command = ReactiveCommand.CreateFromTask(task),
            Icon = icon
        };

        menuItem.Items.Add(toolMenu);
    }

    private static void AddToMenu(ref MenuItem menuItem, string text, AbstractTransformTool tool, Uri icon)
    {
        menuItem.Items ??= [];
        MenuItem toolMenu = new(text)
        {
            Command = new ActionCommand<AbstractTransformTool>(ToolAction),
            CommandParameter = tool,
            Icon = icon
        };

        menuItem.Items.Add(toolMenu);
    }

    private static void ToolAction(AbstractTransformTool tool)
    {
        tool.ApplyTransformation(false, false, false);
    }
}
