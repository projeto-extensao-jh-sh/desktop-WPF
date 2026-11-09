using LMFoods.Manager.Acesso.Abstractions;

namespace LMFoods.Manager.Acesso.Security;

/// <summary>
/// Hash do PIN com BCrypt. O formato gerado ($2a$11$...) é o mesmo usado no seed de testes.
/// </summary>
internal sealed class BCryptPinHasher : IPinHasher
{
    // Custo do BCrypt: cada +1 dobra o tempo do hash. 11 leva cerca de 0,1 s,
    // imperceptível no login e lento para quem tenta testar muitos PINs.
    private const int FatorDeCusto = 11;

    // Gera o hash a ser gravado em pin_hash. Usado no cadastro e na troca de PIN
    public string GerarHash(string pin)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pin);
        return BCrypt.Net.BCrypt.HashPassword(pin, FatorDeCusto);
    }


    // se o PIN digitado corresponde ao hash gravado
    public bool Verificar(string pin, string hash)
    {
        ArgumentNullException.ThrowIfNull(pin);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        // Um hash em formato inválido lança exceção de propósito:
        // é dado corrompido no banco e precisa aparecer, não virar "PIN incorreto".
        return BCrypt.Net.BCrypt.Verify(pin, hash);
    }
}