using System.Windows.Input;

namespace Pixed
{
    internal class ActionCommand<T> : ICommand
    {
        #region Fields
        private readonly Action<T> _action;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public ActionCommand(Action<T> action)
        {
            this._action = action;
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

            _action?.Invoke((T)parameter);
        }

        public void Execute(T parameter)
        {
            _action?.Invoke(parameter);
        }
        #endregion
    }

    internal class ActionCommand : ICommand
    {
        #region Fields
        private readonly Action _action;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public ActionCommand(Action action)
        {
            this._action = action;
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
            Execute();
        }

        public void Execute()
        {
            _action?.Invoke();
        }
        #endregion
    }

    internal class StaticActionCommand<T> : ICommand
    {
        #region Fields
        private readonly Action<T> _action;
        private readonly T _parameter;
        #endregion
        #region Events
        public event EventHandler? CanExecuteChanged;
        #endregion
        #region Constructors
        public StaticActionCommand(Action<T> action, T param)
        {
            this._action = action;
            _parameter = param;
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
            Execute();
        }

        public void Execute()
        {
            _action?.Invoke(_parameter);
        }
        #endregion
    }
}