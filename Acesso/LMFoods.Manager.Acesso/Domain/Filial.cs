namespace LMFoods.Manager.Acesso.Domain;

/// <summary>
/// Unidade da LM Food. Tabela acesso.Filial.
/// </summary>
internal sealed class Filial
{
    public required int Id { get; init; }
    public required string Nome { get; init; }
    public bool Ativo { get; init; } = true;
}