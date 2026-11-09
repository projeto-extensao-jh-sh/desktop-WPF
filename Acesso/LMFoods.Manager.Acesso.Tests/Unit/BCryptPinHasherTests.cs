using LMFoods.Manager.Acesso.Security;

namespace LMFoods.Manager.Acesso.Tests.Unit;

/// <summary>
/// Testes do hash do PIN. Não usam banco, mas cada hash leva cerca de 0,1 s por causa do BCrypt.
/// </summary>
[Trait("Categoria", "Unitario")]
public sealed class BCryptPinHasherTests
{
    // Mesmo hash do LMFoods.Manager.Acesso.Data/Seeds/001_DadosTeste.sql (PIN 1234). Se mudar lá, mude aqui.
    private const string HashDoSeed = "$2a$11$lygi.wgZKCkygMiwyM1KXO/WjMwphJgBsqnyAN9Gy7oIFPFXgIrCO";

    private readonly BCryptPinHasher _hasher = new();

    /// <summary>
    /// O PIN usado para gerar o hash deve ser aceito na conferência.
    /// </summary>
    [Fact]
    public void Verificar_PinCorreto_RetornaTrue()
    {
        var hash = _hasher.GerarHash("1234");

        Assert.True(_hasher.Verificar("1234", hash));
    }

    /// <summary>
    /// Um PIN diferente do usado no hash deve ser recusado.
    /// </summary>
    [Fact]
    public void Verificar_PinIncorreto_RetornaFalse()
    {
        var hash = _hasher.GerarHash("1234");

        Assert.False(_hasher.Verificar("4321", hash));
    }

    /// <summary>
    /// Compara se dois pin tem o mesmo hash
    /// O salt aleatório faz o mesmo PIN gerar hashes diferentes,
    /// então dois usuários com o mesmo PIN não ficam com o mesmo pin_hash.
    /// </summary>
    [Fact]
    public void GerarHash_MesmoPin_GeraHashesDiferentes()
    {
        Assert.NotEqual(_hasher.GerarHash("1234"), _hasher.GerarHash("1234"));
    }

    /// <summary>
    /// Verifica se o PIN não aparece no hash gravado no banco.
    /// </summary>
    [Fact]
    public void GerarHash_NaoContemOPinEmTextoPuro()
    {
        Assert.DoesNotContain("1234", _hasher.GerarHash("1234"));
    }

    /// <summary>
    /// Garante que o hash gravado no seed funciona com o hasher do sistema.
    /// Sem isso, os usuários de teste não conseguiriam fazer login.
    /// </summary>
    [Fact]
    public void Verificar_HashDoSeed_AceitaPin1234()
    {
        Assert.True(_hasher.Verificar("1234", HashDoSeed));
    }
}