using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Windows.Input;
using static Pixed.StaticMenuBuilder;

namespace Pixed.Controls
{
    internal abstract class PixedUserControl : UserControl
    {
        public virtual void RegisterMenuItems()
        {
            if (DataContext is IPixedViewModel model)
            {
                model.RegisterMenuItems();
            }
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            RegisterMenuItems();
            Unloaded += PixedUserControl_Unloaded;
        }

        private void PixedUserControl_Unloaded(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not IDisposable viewModel) return;
            DataContext = null;
            viewModel.Dispose();
        }

        public static void RegisterMenuItem(BaseMenuItem baseMenu, string text, Action action)
        {
            RegisterMenuItem(baseMenu, text, new ActionCommand(action));
        }

        public static void RegisterMenuItem(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
        {
            RegisterMenuItem(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
        }

        public static void RegisterMenuItem(BaseMenuItem baseMenu, NativeMenuItem menuItem)
        {
            StaticMenuBuilder.AddEntry(baseMenu, menuItem);
        }
    }
}
