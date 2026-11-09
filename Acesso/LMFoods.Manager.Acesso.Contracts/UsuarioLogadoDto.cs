namespace LMFoods.Manager.Acesso.Contracts;

/// <summary>
/// Usuário autenticado, guardado na sessão do app após o login.
/// </summary>
public sealed record UsuarioLogadoDto(
    int Id,
    string Nome,
    string? Cargo,
    int IdFilial,
    string Filial,
    int? IdCaixa,
    string? Caixa,
    int IdSessao
);