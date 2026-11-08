using LMFoods.Manager.Wpf.Commands;
using System.Windows.Input;

namespace LMFoods.Manager.Wpf.ViewModels;

public class LoginViewModel : ViewModelBase
{
    public LoginViewModel()
    {
        loginCommand = new RelayCommand(OpenMainWindow, CanExecuteMethod);
    }

    private void OpenMainWindow(object? obj)
    {

    }



    // Copiar Command -------------------------------------------------------------------
    private ICommand loginCommand;
    public ICommand LoginCommand
    {
        get => loginCommand;
        set => loginCommand = value;
    }

    private bool CanExecuteMethod(object? parameter)
    {
        return true;
    }
}
