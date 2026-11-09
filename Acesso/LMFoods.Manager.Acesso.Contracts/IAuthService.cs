namespace LMFoods.Manager.Acesso.Contracts;

/// <summary>
/// Autenticação em duas etapas: o usuário digita o login e depois o PIN.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// (LoginView): busca o usuário pelo login para exibir nome e cargo na tela do PIN.
    /// Retorna <c>null</c> se o login não existir ou o usuário estiver inativo.
    /// </summary>
    Task<UsuarioIdentificadoDto?> IdentificarAsync(string login, CancellationToken ct = default);

    /// <summary>
    /// (LoginPinView): valida o PIN e, se aceito, abre uma sessão.
    /// </summary>
    Task<ResultadoLogin> AutenticarAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Registra o logout, preenchendo a data/hora de saída da sessão.
    /// </summary>
    Task EncerrarSessaoAsync(int idSessao, CancellationToken ct = default);
}