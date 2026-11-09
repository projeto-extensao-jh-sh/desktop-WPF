using LMFoods.Manager.Acesso.Domain;

namespace LMFoods.Manager.Acesso.Abstractions;

/// <summary>
/// Acesso aos usuários. Implementado no Acesso.Data (Sql e Mock).
/// </summary>
internal interface IUsuarioRepository
{
    /// <summary>
    /// Busca o usuário pelo login, com a filial e o caixa preenchidos.
    /// Retorna null se o login não existir.
    /// </summary>
    Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken ct = default);

    /// <summary>
    /// Busca o usuário pelo id, com a filial e o caixa preenchidos.
    /// Retorna null se o id não existir.
    /// </summary>
    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Grava as tentativas erradas e o bloqueio atuais do usuário.
    /// </summary>
    Task SalvarTentativasAsync(Usuario usuario, CancellationToken ct = default);
}