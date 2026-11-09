namespace LMFoods.Manager.Acesso.Contracts;

/// <summary>Dados enviados pela tela do PIN</summary>
public sealed record LoginRequest(int IdUsuario, string Pin)
{
    // O ToString gerado pelo record incluiria o PIN em texto puro.
    // Isso evita que ele vaze em logs ou no depurador.
    public override string ToString() => $"LoginRequest {{ IdUsuario = {IdUsuario}, Pin = *** }}";
}