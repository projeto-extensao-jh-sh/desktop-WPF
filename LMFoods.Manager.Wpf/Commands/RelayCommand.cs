using System.Windows.Input;

namespace LMFoods.Manager.Wpf.Commands;

public class RelayCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    private readonly Action<object?>? _executeMethod;
    private readonly Func<object?, bool>? _canExecuteMethod;

    private readonly Action<bool>? _executeBool;
    private readonly Func<bool>? _canExecuteBool;

    // Construtor para Action<object?>
    public RelayCommand(Action<object?> executeMethod, Func<object?, bool>? canExecuteMethod = null)
    {
        _executeMethod = executeMethod;
        _canExecuteMethod = canExecuteMethod;
    }

    // Construtor para Action<bool>
    public RelayCommand(Action<bool> executeBool, Func<bool>? canExecuteBool = null)
    {
        _executeBool = executeBool;
        _canExecuteBool = canExecuteBool;
    }

    // Condição para execução
    public bool CanExecute(object? parameter)
    {
        if (parameter is bool)
            return _canExecuteBool == null || _canExecuteBool();

        return _canExecuteMethod == null || _canExecuteMethod(parameter);
    }

    // Lógica de execução
    public void Execute(object? parameter)
    {
        if (parameter is bool boolParam)
            _executeBool?.Invoke(boolParam);
        else
            _executeMethod?.Invoke(parameter);
    }

    // Notifica a UI (ex: botões) para reavaliar CanExecute
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

// Classe genérica RelayCommand<T> para suporte a tipos específicos como DragEventArgs e KeyEventArgs
public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || (parameter is T typedParam && _canExecute(typedParam));
    }

    public void Execute(object? parameter)
    {
        if (parameter is T typedParam)
            _execute(typedParam);
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}