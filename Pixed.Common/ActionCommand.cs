using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Common;

public class ActionCommand<T> : ICommand
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
        _action = action;
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

public class ActionCommand : ICommand
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
        _action = action;
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

public class StaticActionCommand<T> : ICommand
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
        _action = action;
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

public class AsyncCommand<T> : ICommand
{
    #region Fields
    private readonly Func<T, Task> _func;
    #endregion
    #region Events
    public event EventHandler? CanExecuteChanged;
    #endregion
    #region Constructors
    public AsyncCommand(Func<T, Task> func)
    {
        _func = func;
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

        Execute((T)parameter);
    }

    public async Task Execute(T parameter)
    {
        await _func?.Invoke(parameter);
    }
    #endregion
}

public class AsyncCommand : ICommand
{
    #region Fields
    private readonly Func<Task> _func;
    #endregion
    #region Events
    public event EventHandler? CanExecuteChanged;
    #endregion
    #region Constructors
    public AsyncCommand(Func<Task> func)
    {
        _func = func;
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

    public async Task Execute()
    {
        await _func?.Invoke();
    }
    #endregion
}