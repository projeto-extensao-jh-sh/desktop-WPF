using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopApp.Views;

namespace DesktopApp.View
{
    public partial class PanelControlView : Window
    {
        public PanelControlView()
        {
            InitializeComponent();
        }

        public PanelControlView(string usuarioId, string nome, string cargo, string iniciais)
            : this()
        {
            txtNomeUsuario.Text = nome;
            txtCargoUsuario.Text = cargo;
            txtIniciais.Text = iniciais;
        }

        private void btnSair_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AbrirModulo_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                if (tb.Tag?.ToString() == "Mantenimiento")
                {
                    var cadastroUsuario = new CadastroUsuarioView
                    {
                        Owner = this
                    };

                    cadastroUsuario.ShowDialog();
                    return;
                }

                MessageBox.Show($"Abrindo módulo: {tb.Tag}");
            }
        }
    }
}