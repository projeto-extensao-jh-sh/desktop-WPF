namespace LMFoods.Manager.Acesso.Contracts;

/// <summary>
/// Usuário encontrado pelo login, exibido na tela do PIN. Não contém dados sensíveis.
/// </summary>
public sealed record UsuarioIdentificadoDto(
    int Id,
    string Nome,
    string? Cargo
);