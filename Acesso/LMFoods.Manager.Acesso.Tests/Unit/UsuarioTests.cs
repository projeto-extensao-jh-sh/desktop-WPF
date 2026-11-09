using LMFoods.Manager.Acesso.Domain;

namespace LMFoods.Manager.Acesso.Tests.Unit;

/// <summary>
/// Regras de tentativas e bloqueio do PIN, com a política padrão (5 tentativas, 5 minutos).
/// Testes de unidade: não usam banco, só a entidade Usuario em memória.
/// </summary>
[Trait("Categoria", "Unitario")]
public sealed class UsuarioTests
{
    // Momento fixo usado como "agora" em todos os testes.
    // Com uma data fixa, o resultado é sempre o mesmo, em qualquer dia em que o teste rodar.
    private static readonly DateTime Agora = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    // Política padrão do sistema: 5 tentativas erradas bloqueiam por 5 minutos
    private static readonly PoliticaBloqueio Politica = PoliticaBloqueio.Padrao;

    /// <summary>
    /// Um PIN errado conta uma tentativa, mas ainda não bloqueia.
    /// </summary>
    [Fact]
    public void RegistrarPinIncorreto_AbaixoDoLimite_SomaTentativaSemBloquear()
    {
        // Arrange: usuário sem nenhuma tentativa errada
        var usuario = CriarUsuario();

        // Act: primeiro PIN errado
        usuario.RegistrarPinIncorreto(Agora, Politica);

        // Assert: contou 1 tentativa, não bloqueou e restam 4 (5 - 1)
        Assert.Equal(1, usuario.TentativasFalhas);
        Assert.False(usuario.EstaBloqueado(Agora));
        Assert.Equal(4, usuario.TentativasRestantes(Politica));
    }

    /// <summary>
    /// A quinta tentativa errada bloqueia o usuário por 5 minutos.
    /// </summary>
    [Fact]
    public void RegistrarPinIncorreto_AtingeLimite_BloqueiaEZeraTentativas()
    {
        // Arrange: usuário que já errou 4 vezes
        var usuario = CriarUsuario(tentativasFalhas: 4);

        // Act: quinta tentativa errada, que atinge o limite
        usuario.RegistrarPinIncorreto(Agora, Politica);

        // Assert: ficou bloqueado até exatamente 5 minutos depois de agora
        Assert.True(usuario.EstaBloqueado(Agora));
        Assert.Equal(Agora.AddMinutes(5), usuario.BloqueadoAte);

        // Assert: o contador zera, para o usuário ter 5 tentativas novas quando o bloqueio acabar
        Assert.Equal(0, usuario.TentativasFalhas);
    }

    /// <summary>
    /// Depois que o tempo de bloqueio passa, o usuário pode tentar de novo.
    /// </summary>
    [Fact]
    public void EstaBloqueado_DepoisDoFimDoBloqueio_RetornaFalse()
    {
        // Arrange: usuário bloqueado até 5 minutos depois de agora
        var usuario = CriarUsuario(bloqueadoAte: Agora.AddMinutes(5));

        // Act + Assert: 6 minutos depois, o bloqueio já expirou.
        // Como a hora é um parâmetro, o teste simula o futuro sem esperar de verdade.
        Assert.False(usuario.EstaBloqueado(Agora.AddMinutes(6)));
    }

    /// <summary>
    /// Um login aceito limpa as tentativas e qualquer bloqueio anterior.
    /// </summary>
    [Fact]
    public void RegistrarLoginComSucesso_ZeraTentativasEBloqueio()
    {
        // Arrange: usuário com 3 erros e um bloqueio antigo, que já expirou
        var usuario = CriarUsuario(tentativasFalhas: 3, bloqueadoAte: Agora.AddMinutes(-1));

        // Act: acertou o PIN
        usuario.RegistrarLoginComSucesso();

        // Assert: volta ao estado limpo, como um usuário que nunca errou
        Assert.Equal(0, usuario.TentativasFalhas);
        Assert.Null(usuario.BloqueadoAte);
    }

    /// <summary>
    /// Cria um usuário válido, alterando só o que o teste precisa.
    /// Os parâmetros opcionais deixam cada teste mostrar apenas o que importa para ele.
    /// </summary>
    private static Usuario CriarUsuario(int tentativasFalhas = 0, DateTime? bloqueadoAte = null) => new()
    {
        // Campos obrigatórios (required) com valores quaisquer: não influenciam as regras testadas
        Id = 1,
        Login = "ana",
        Nome = "Ana Carvalho",
        PinHash = "hash",
        Filial = new Filial { Id = 1, Nome = "Maderera Bozovich" },

        // Estado inicial de tentativas e bloqueio, definido por cada teste
        TentativasFalhas = tentativasFalhas,
        BloqueadoAte = bloqueadoAte
    };
}