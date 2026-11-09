using System.Diagnostics.CodeAnalysis;

namespace LMFoods.Manager.Acesso.Contracts;

/// <summary>
/// Resultado de uma tentativa de login.
/// </summary>
public sealed record ResultadoLogin
{
    /// <summary>Usuário autenticado. Preenchido apenas quando o login foi aceito.</summary>
    public UsuarioLogadoDto? Usuario { get; init; }

    /// <summary>Motivo da recusa. Preenchido apenas quando o login falhou.</summary>
    public MotivoFalhaLogin? Motivo { get; init; }

    /// <summary>Tentativas restantes antes do bloqueio, quando o PIN está incorreto.</summary>
    public int? TentativasRestantes { get; init; }

    /// <summary>Até quando o usuário fica bloqueado (UTC), quando o motivo é bloqueio.</summary>
    public DateTimeOffset? BloqueadoAte { get; init; }

    // Quando Sucesso for true, o Usuario não é nulo
    [MemberNotNullWhen(true, nameof(Usuario))]
    // Quando Sucesso for false, o Motivo não é nulo
    [MemberNotNullWhen(false, nameof(Motivo))]
    public bool Sucesso => Usuario is not null;


    // Cada resultado cria um combinação com os campos corretos 
    #region Construtores de resultados    
    public static ResultadoLogin Ok(UsuarioLogadoDto usuario) =>
        new() { Usuario = usuario };


    public static ResultadoLogin PinIncorreto(int tentativasRestantes) =>
        new() { Motivo = MotivoFalhaLogin.PinIncorreto, TentativasRestantes = tentativasRestantes };

    public static ResultadoLogin Inativo() =>
        new() { Motivo = MotivoFalhaLogin.UsuarioInativo };

    public static ResultadoLogin Bloqueado(DateTimeOffset ate) =>
        new() { Motivo = MotivoFalhaLogin.UsuarioBloqueado, BloqueadoAte = ate };
    #endregion
}