namespace LMFoods.Manager.Acesso.Domain;

/// <summary>
/// Um login e seu logout. Tabela acesso.SessaoUsuario.
/// As datas são gravadas pelo banco, em UTC, para todas as filiais usarem o mesmo relógio.
/// </summary>
internal sealed class SessaoUsuario
{
    public required int Id { get; init; }
    public required int UsuarioId { get; init; }
    public int? CaixaId { get; init; }
    public required DateTime DataHoraLogin { get; init; }
    public DateTime? DataHoraLogout { get; init; }

    public bool Aberta => DataHoraLogout is null;
}