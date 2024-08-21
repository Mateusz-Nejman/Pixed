using System.Windows.Input;

namespace Pixed
{
    internal class ActionCommand<T> : ICommand
    {
        #region Fields
        private readonly Action<T> action;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public ActionCommand(Action<T> action)
        {
            this.action = action;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        #region Public Methods
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                return;
            }

            action?.Invoke((T)parameter);
        }

        public void Execute(T parameter)
        {
            action?.Invoke(parameter);
        }
        #endregion
    }

    internal class ActionCommand : ICommand
    {
        #region Fields
        private readonly Action action;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public ActionCommand(Action action)
        {
            this.action = action;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        #region Public Methods
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action?.Invoke();
        }
        #endregion
    }
}