using System.Windows;

namespace DesktopApp.Views;

public partial class CadastroUsuarioView : Window
{
    public CadastroUsuarioView()
    {
        InitializeComponent();
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Salvar_Click(object sender, RoutedEventArgs e)
    {
        txtMensagem.Foreground = System.Windows.Media.Brushes.Firebrick;

        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            ExibirErro("Informe o nome completo.");
            txtNome.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtLogin.Text))
        {
            ExibirErro("Informe o usuário de acesso.");
            txtLogin.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtSenha.Password))
        {
            ExibirErro("Informe uma senha.");
            txtSenha.Focus();
            return;
        }

        if (txtSenha.Password.Length < 6)
        {
            ExibirErro("A senha deve ter pelo menos 6 caracteres.");
            txtSenha.Focus();
            return;
        }

        if (txtSenha.Password != txtConfirmarSenha.Password)
        {
            ExibirErro("A confirmação de senha não confere.");
            txtConfirmarSenha.Focus();
            return;
        }

        txtMensagem.Foreground = System.Windows.Media.Brushes.ForestGreen;
        txtMensagem.Text = "Dados validados com sucesso. O cadastro será salvo quando o backend estiver conectado.";
    }

    private void ExibirErro(string mensagem)
    {
        txtMensagem.Text = mensagem;
    }
}
