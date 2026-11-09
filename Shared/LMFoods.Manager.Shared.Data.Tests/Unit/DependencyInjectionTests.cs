using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMFoods.Manager.Shared.Data.Tests.Unit;

/// <summary>
/// Testes do AddSharedData. Não usam o banco, só conferem o que foi registrado.
/// </summary>
[Trait("Categoria", "Unitario")]
public sealed class DependencyInjectionTests
{
    /// <summary>
    /// Se o appsettings.json não tiver a seção "Data", o app tem que abrir com o Mock
    /// e sem a conexão com o banco.
    /// </summary>
    [Fact]
    public void AddSharedData_SemSecaoData_UsaMockENaoRegistraConexao()
    {
        var services = new ServiceCollection();

        services.AddSharedData(CriarConfiguracao(new()));

        var opcoes = PegarOpcoes(services);
        Assert.Equal(ProvedorDados.Mock, opcoes.Provedor);
        Assert.False(opcoes.IncluirDadosTeste);
        Assert.False(FoiRegistrado<SqlConnectionFactory>(services));
    }


    /// <summary>
    /// Com o provedor Sql e a connection string preenchida, os valores do json
    /// têm que chegar no DataOptions e a conexão tem que ser registrada.
    /// </summary>
    [Fact]
    public void AddSharedData_ComProvedorSql_LeConfiguracaoERegistraConexao()
    {
        var services = new ServiceCollection();
        var configuracao = CriarConfiguracao(new()
        {
            ["Data:Provedor"] = "Sql",
            ["Data:ConnectionString"] = "Server=x;Database=y;",
            ["Data:IncluirDadosTeste"] = "true"
        });

        services.AddSharedData(configuracao);

        var opcoes = PegarOpcoes(services);
        Assert.Equal(ProvedorDados.Sql, opcoes.Provedor);
        Assert.Equal("Server=x;Database=y;", opcoes.ConnectionString);
        Assert.True(opcoes.IncluirDadosTeste);
        Assert.True(FoiRegistrado<SqlConnectionFactory>(services));
    }


    /// <summary>
    /// Provedor Sql sem connection string tem que dar erro já na abertura do app.
    /// </summary>
    [Fact]
    public void AddSharedData_SqlSemConnectionString_LancaExcecao()
    {
        var services = new ServiceCollection();
        var configuracao = CriarConfiguracao(new() { ["Data:Provedor"] = "Sql" });

        Assert.Throws<InvalidOperationException>(() => services.AddSharedData(configuracao));
    }


    /// <summary>
    /// Sem configuração nenhuma (null) também tem que dar erro na hora.
    /// </summary>
    [Fact]
    public void AddSharedData_ConfiguracaoNula_LancaExcecao()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() => services.AddSharedData(null!));
    }


    // Monta um appsettings.json "de mentira" na memória, sem precisar de arquivo.
    // O ":" separa a seção da propriedade (Data:Provedor = "Data": { "Provedor": ... })
    private static IConfiguration CriarConfiguracao(Dictionary<string, string?> valores) =>
        new ConfigurationBuilder().AddInMemoryCollection(valores).Build();


    // Procura na lista de registros o DataOptions que o AddSharedData guardou
    private static DataOptions PegarOpcoes(IServiceCollection services) =>
        (DataOptions)services.Single(s => s.ServiceType == typeof(DataOptions)).ImplementationInstance!;


    // Confere se alguma classe do tipo T foi registrada
    private static bool FoiRegistrado<T>(IServiceCollection services) =>
        services.Any(s => s.ServiceType == typeof(T));
}