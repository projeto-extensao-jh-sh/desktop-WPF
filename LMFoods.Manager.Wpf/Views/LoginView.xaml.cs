using System.Windows;
using System.Windows.Controls;

namespace LMFoods.Manager.Wpf.View
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        // Dispara ao clicar em qualquer card de usuário.
        // O Tag do botão traz "Id|Nome|Cargo|Iniciais" separados por "|".
        private void BtnUsuario_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var dados = btn.Tag?.ToString()?.Split('|');
            if (dados == null || dados.Length < 4) return;

            string id = dados[0];
            string nome = dados[1];
            string cargo = dados[2];
            string iniciais = dados[3];

            var telaPin = new LoginPinView(id, nome, cargo, iniciais);
            telaPin.Show();
            Close();
        }
    }
}