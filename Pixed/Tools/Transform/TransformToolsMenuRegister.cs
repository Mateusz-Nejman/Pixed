using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Models;
using Pixed.Windows;
using System;
using static Pixed.MenuBuilder;
using System.Windows.Input;
using Pixed.Selection;

namespace Pixed.Tools.Transform
{
    internal class TransformToolsMenuRegister(MenuBuilder menuBuilder, SelectionManager selectionManager, ApplicationData applicationData)
    {
        private readonly MenuBuilder _menuBuilder = menuBuilder;
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

            RegisterMenuItem(BaseMenuItem.Tools, transformMenu);
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

        private void RegisterMenuItem(BaseMenuItem baseMenu, string text, Action action)
        {
            RegisterMenuItem(baseMenu, text, new ActionCommand(action));
        }

        private void RegisterMenuItem(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
        {
            RegisterMenuItem(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
        }

        private void RegisterMenuItem(BaseMenuItem baseMenu, NativeMenuItem menuItem)
        {
            _menuBuilder.AddEntry(baseMenu, menuItem); //TODO move to separate class
        }
    }
}
