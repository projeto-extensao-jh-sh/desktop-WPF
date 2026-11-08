using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LMFoods.Manager.Wpf.View
{
    public partial class LoginPinView : Window
    {
        // TODO: substituir por validação real (ViewModel / serviço de autenticação)
        private const string SENHA_TESTE = "1234";

        private string _senhaDigitada = string.Empty;
        private readonly string _usuarioId = string.Empty;

        // Construtor padrão (mantido para o Designer / uso avulso)
        public LoginPinView()
        {
            InitializeComponent();
        }

        // Construtor usado ao vir da tela de seleção de usuário
        public LoginPinView(string usuarioId, string nome, string cargo, string iniciais) : this()
        {
            _usuarioId = usuarioId;
            txtNomeUsuario.Text = nome;
            txtCargoUsuario.Text = cargo;
            txtIniciais.Text = iniciais;
        }

        private void BtnNumero_Click(object sender, RoutedEventArgs e)
        {
            if (_senhaDigitada.Length >= 4) return;

            var btn = (Button)sender;
            _senhaDigitada += btn.Tag.ToString();

            AtualizarBolinhas();

            if (_senhaDigitada.Length == 4)
            {
                ValidarSenha();
            }
        }

        private void BtnApagar_Click(object sender, RoutedEventArgs e)
        {
            if (_senhaDigitada.Length == 0) return;

            _senhaDigitada = _senhaDigitada.Substring(0, _senhaDigitada.Length - 1);
            AtualizarBolinhas();
            ResetarEstadoErro();
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            _senhaDigitada = string.Empty;
            AtualizarBolinhas();
            ResetarEstadoErro();
        }

        private void AtualizarBolinhas()
        {
            var dots = new[] { dot1, dot2, dot3, dot4 };
            var corAtiva = ObterCorOuPadrao(this, "CorDotAtivo", "#B23A3A");
            var corInativa = ObterCorOuPadrao(this, "CorDotInativo", "#E0DEDD");

            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].Fill = i < _senhaDigitada.Length ? corAtiva : corInativa;
            }
        }

        private void ValidarSenha()
        {
            if (_senhaDigitada == SENHA_TESTE)
            {
                MostrarSucesso();
            }
            else
            {
                MostrarErro();
            }
        }

        // Busca um recurso de cor pelo nome; se não existir no XAML, usa a cor padrão em vez de travar
        private static Brush ObterCorOuPadrao(FrameworkElement elemento, string chave, string corHexPadrao)
        {
            var recurso = elemento.TryFindResource(chave);
            if (recurso is Brush brush)
                return brush;

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(corHexPadrao));
        }

        private void MostrarSucesso()
        {
            var corSucesso = ObterCorOuPadrao(this, "CorSucesso", "#2FAE60");

            foreach (var dot in new[] { dot1, dot2, dot3, dot4 })
            {
                dot.Fill = corSucesso;
            }

            txtInstrucao.Text = "Clave correcta";
            txtInstrucao.Foreground = corSucesso;

            this.IsHitTestVisible = false;

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(700)
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop();

                // Abre o painel principal
                PanelControlView painel = new PanelControlView(
                    _usuarioId,
                    txtNomeUsuario.Text,
                    txtCargoUsuario.Text,
                    txtIniciais.Text);

                painel.Show();

                this.Close();
            };

            timer.Start();
        }


        private void MostrarErro()
        {
            var corErro = ObterCorOuPadrao(this, "CorErro", "#D62E2E");
            foreach (var dot in new[] { dot1, dot2, dot3, dot4 })
            {
                dot.Fill = corErro;
            }

            txtInstrucao.Text = "Clave incorrecta, intente nuevamente";
            txtInstrucao.Foreground = corErro;

            // Limpa depois de um instante para o usuário tentar de novo
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(600)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                _senhaDigitada = string.Empty;
                AtualizarBolinhas();
                ResetarEstadoErro();
            };
            timer.Start();
        }

        private void ResetarEstadoErro()
        {
            txtInstrucao.Text = "Ingrese su clave de 4 dígitos";
            txtInstrucao.Foreground = ObterCorOuPadrao(this, "CorSubtitulo", "#8C8C99");
        }

        private void btnCambiarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // TODO: navegar de volta para a tela de seleção de usuário (LoginView)
            var login = new LoginView();
            login.Show();
            Close();
        }

        private void LinkAdministrador_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: abrir tela/contato do administrador
        }
    }
}