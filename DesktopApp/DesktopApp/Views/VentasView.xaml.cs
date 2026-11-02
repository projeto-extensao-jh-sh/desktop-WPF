using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DesktopApp.View
{
    public partial class VentasView : Window
    {
        private readonly DispatcherTimer _timerRelogio;
        private DispatcherTimer? _timerScroll;
        private double _scrollAlvo;

        // Largura aproximada de um card (200) + margem direita (16)
        private const double LarguraCard = 216;

        private string _usuarioId = string.Empty;
        private string _nomeUsuario = string.Empty;
        private string _cargoUsuario = string.Empty;
        private string _iniciaisUsuario = string.Empty;

        public VentasView()
        {
            InitializeComponent();

            _timerRelogio = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timerRelogio.Tick += (s, e) => AtualizarRelogio();
            _timerRelogio.Start();
            AtualizarRelogio();
        }

        public VentasView(string usuarioId, string nome, string cargo, string iniciais) : this()
        {
            _usuarioId = usuarioId;
            _nomeUsuario = nome;
            _cargoUsuario = cargo;
            _iniciaisUsuario = iniciais;

            txtNomeUsuario.Text = nome;
            txtCargoUsuario.Text = cargo;
            txtIniciais.Text = iniciais;
        }

        private void AtualizarRelogio()
        {
            var agora = DateTime.Now;
            txtRelogio.Text = agora.ToString("HH:mm:ss");

            var cultura = new System.Globalization.CultureInfo("es-ES");
            string texto = agora.ToString("dddd, dd 'De' MMMM 'De' yyyy", cultura);
            txtData.Text = cultura.TextInfo.ToTitleCase(texto);
        }

        // ===================== CARROSSEL =====================

        private void BtnScrollEsquerda_Click(object sender, RoutedEventArgs e) => ScrollSuave(-LarguraCard * 2);

        private void BtnScrollDireita_Click(object sender, RoutedEventArgs e) => ScrollSuave(LarguraCard * 2);

        private void ScrollSuave(double delta)
        {
            double max = carrossel.ScrollableWidth;
            _scrollAlvo = Math.Max(0, Math.Min(carrossel.HorizontalOffset + delta, max));

            _timerScroll?.Stop();
            _timerScroll = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(15) };
            _timerScroll.Tick += (s, e) =>
            {
                double atual = carrossel.HorizontalOffset;
                double diferenca = _scrollAlvo - atual;

                if (Math.Abs(diferenca) < 1)
                {
                    carrossel.ScrollToHorizontalOffset(_scrollAlvo);
                    _timerScroll?.Stop();
                    return;
                }

                // Easing suave: percorre 25% da distância restante a cada tick
                carrossel.ScrollToHorizontalOffset(atual + diferenca * 0.25);
            };
            _timerScroll.Start();
        }

        // ===================== RECEITAS =====================

        private void Receita_Click(object sender, MouseButtonEventArgs e)
        {
            var card = (Border)sender;
            string receita = card.Tag?.ToString() ?? string.Empty;

            // TODO: abrir a tela de registro do pedido com a receita selecionada
            MessageBox.Show(this, $"Receta seleccionada: {receita}", "Registro de Venta");
        }

        // ===================== NAVEGAÇÃO =====================

        private void BreadcrumbInicio_Click(object sender, MouseButtonEventArgs e)
        {
            _timerRelogio.Stop();
            _timerScroll?.Stop();

            var painel = new PanelControlView(_usuarioId, _nomeUsuario, _cargoUsuario, _iniciaisUsuario);
            painel.Show();
            Close();
        }

        private void btnSair_Click(object sender, RoutedEventArgs e)
        {
            _timerRelogio.Stop();
            _timerScroll?.Stop();

            var login = new LoginView();
            login.Show();
            Close();
        }
    }
}