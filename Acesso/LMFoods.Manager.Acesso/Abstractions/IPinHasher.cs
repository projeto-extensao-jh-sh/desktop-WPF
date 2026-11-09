namespace LMFoods.Manager.Acesso.Abstractions;

/// <summary>
/// Gera e confere o hash do PIN. O PIN nunca é gravado em texto puro.
/// </summary>
internal interface IPinHasher
{
    /// <summary>Gera o hash a ser gravado em pin_hash. Usado no cadastro e na troca de PIN.</summary>
    string GerarHash(string pin);

    /// <summary>Confere se o PIN digitado corresponde ao hash gravado.</summary>
    bool Verificar(string pin, string hash);
}