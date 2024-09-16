using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Tools.Transform;
using System.Windows.Input;

namespace Pixed.ViewModels;

internal class TransformSectionViewModel : PropertyChangedBase, IPixedViewModel
{
    public ICommand ToolCommand { get; }

    public AbstractTransformTool ToolFlip { get; } = new Flip();
    public AbstractTransformTool ToolRotate { get; } = new Rotate();
    public AbstractTransformTool ToolCenter { get; } = new Center();
    public AbstractTransformTool ToolCrop { get; } = new Crop();

    public TransformSectionViewModel()
    {
        ToolCommand = new ActionCommand<AbstractTransformTool>(ToolAction);
    }

    public void RegisterMenuItems()
    {
        NativeMenuItem transformMenu = new("Transform")
        {
            Menu = []
        };
        AddToMenu(ref transformMenu, "Flip horizontaly", ToolFlip);
        AddToMenu(ref transformMenu, "Counter-clockwise rotation", ToolRotate);
        AddToMenu(ref transformMenu, "Align image to the center", ToolCenter);
        AddToMenu(ref transformMenu, "Crop to fit the content or the selection", ToolCrop);
        //TODO Ctrl Shift, Alt variants

        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, transformMenu);
    }

    private void AddToMenu(ref NativeMenuItem menuItem, string text, AbstractTransformTool tool)
    {
        menuItem.Menu ??= [];
        NativeMenuItem toolMenu = new(text)
        {
            Command = ToolCommand,
            CommandParameter = tool
        };

        menuItem.Menu.Add(toolMenu);
    }

    private void ToolAction(AbstractTransformTool tool)
    {
        tool.ApplyTransformation();
    }
}
