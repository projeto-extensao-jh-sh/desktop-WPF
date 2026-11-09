namespace LMFoods.Manager.Acesso.Abstractions;

/// <summary>
/// Registro de login e logout. Implementado no Acesso.Data (Sql e Mock).
/// As datas são definidas pelo banco, em UTC.
/// </summary>
internal interface ISessaoUsuarioRepository
{
    /// <summary>
    /// Abre uma sessão e devolve o id dela.
    /// </summary>
    Task<int> AbrirAsync(int usuarioId, int? caixaId, CancellationToken ct = default);

    /// <summary>
    /// Registra o logout da sessão.
    /// Retorna false se a sessão não existir ou já estiver encerrada.
    /// </summary>
    Task<bool> EncerrarAsync(int idSessao, CancellationToken ct = default);
}