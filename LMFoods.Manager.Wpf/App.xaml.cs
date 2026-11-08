using LMFoods.Manager.Wpf.View;
using LMFoods.Manager.Wpf.ViewModels;
using System.Windows;
using System.Windows.Input;


namespace LMFoods.Manager.Wpf
{
    public partial class App : Application
    {
        // ################ START PROCESS ################
        /// <summary>
        /// Controla a inicialização do aplicativo, verificando a validade da licença antes de exibir a janela principal.
        /// Abre a janela principal do aplicativo, configurando o DataContext com o StartViewModel e seus serviços dependentes.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var login = new LoginView
            {
                DataContext = new LoginViewModel()
            };

            Application.Current.MainWindow = login;
            login.Show();
        }


        /// <summary>
        /// Ação para permitir arrastar a janela clicando em qualquer área dela.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Window window)
                    window.DragMove();
            }
        }


        private void Click_QuestionButton(object sender, RoutedEventArgs e)
        {
            //string teste = "Question";
        }
    }
}