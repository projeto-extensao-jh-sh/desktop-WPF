using LMFoods.Manager.Acesso.Abstractions;
using LMFoods.Manager.Acesso.Contracts;
using LMFoods.Manager.Acesso.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace LMFoods.Manager.Acesso.Tests.Unit;

/// <summary>
/// Testes do AddAcesso. Os repositórios são falsos, porque quem registra os de verdade é o AddAcessoData.
/// </summary>
[Trait("Categoria", "Unitario")]
public sealed class DependencyInjectionTests
{
    /// <summary>
    /// Com os repositórios registrados, o app consegue pedir um IAuthService pronto.
    /// </summary>
    [Fact]
    public void AddAcesso_ComRepositorios_CriaAuthService()
    {
        // Faz o mesmo que o app vai fazer, só que em miniatura: cria uma lista vazia, registra os repositórios falsos, chama o AddAcesso e, no fim, o BuildServiceProvider() transforma a lista num provedor
        var provedor = new ServiceCollection()
            .AddSingleton<IUsuarioRepository, UsuarioRepositorioFalso>()
            .AddSingleton<ISessaoUsuarioRepository, SessaoRepositorioFalso>()
            .AddAcesso()
            .BuildServiceProvider();

        // Pede o serviço do login: se faltar alguma peça, dá erro aqui
        Assert.NotNull(provedor.GetRequiredService<IAuthService>());
    }

    /// <summary>
    /// Registra o relógio do teste antes de chamar o AddAcesso, para ver se ele continua lá depois
    /// </summary>
    [Fact]
    public void AddAcesso_ComRelogioJaRegistrado_MantemORelogio()
    {
        var relogioDoTeste = new RelogioFalso();

        // Faz o mesmo que o app vai fazer, só que em miniatura: cria uma lista vazia, registra os repositórios falsos, chama o AddAcesso e, no fim, o BuildServiceProvider() transforma a lista num provedor
        var provedor = new ServiceCollection()
            .AddSingleton<TimeProvider>(relogioDoTeste)
            .AddAcesso()
            .BuildServiceProvider();

        // Tem que ser o mesmo objeto, e não o relógio do sistema
        Assert.Same(relogioDoTeste, provedor.GetRequiredService<TimeProvider>());
    }

    /// <summary>
    /// Classes falsas só para o teste montar o AuthService. Nenhum método é chamado.
    /// Repositório de usuários falso: só existe para o AuthService poder ser montado no teste
    /// </summary>
    private sealed class UsuarioRepositorioFalso : IUsuarioRepository
    {
        public Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken ct = default) => throw new NotImplementedException();
        public Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default) => throw new NotImplementedException();
        public Task SalvarTentativasAsync(Usuario usuario, CancellationToken ct = default) => throw new NotImplementedException();
    }


    /// <summary>
    /// Sessão falsa: só existe para o AuthService poder ser montado no teste
    /// </summary>
    private sealed class SessaoRepositorioFalso : ISessaoUsuarioRepository
    {
        public Task<int> AbrirAsync(int usuarioId, int? caixaId, CancellationToken ct = default) => throw new NotImplementedException();
        public Task<bool> EncerrarAsync(int idSessao, CancellationToken ct = default) => throw new NotImplementedException();
    }

    // Relógio vazio, só para ser diferente do relógio do sistema
    private sealed class RelogioFalso : TimeProvider;
}