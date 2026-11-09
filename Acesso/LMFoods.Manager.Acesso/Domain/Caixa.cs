namespace LMFoods.Manager.Acesso.Domain;

/// <summary>
/// Ponto de venda (balcão) de uma filial. Tabela acesso.Caixa.
/// </summary>
internal sealed class Caixa
{
    public required int Id { get; init; }
    public required int FilialId { get; init; }
    public required string Nome { get; init; }
    public bool Ativo { get; init; } = true;
}
