namespace LMFoods.Manager.Acesso.Domain;

/// <summary>
/// Regras de bloqueio do usuário após PINs incorretos.
/// </summary>
internal sealed record PoliticaBloqueio(int MaxTentativas, TimeSpan DuracaoBloqueio)
{
    /// <summary>5 tentativas erradas bloqueiam o usuário por 5 minutos.</summary>
    public static PoliticaBloqueio Padrao { get; } = new(5, TimeSpan.FromMinutes(5));
}
