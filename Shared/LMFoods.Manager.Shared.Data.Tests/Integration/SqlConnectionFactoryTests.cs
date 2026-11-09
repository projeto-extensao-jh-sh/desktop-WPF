using System.Data;

namespace LMFoods.Manager.Shared.Data.Tests.Integration;

/// <summary>
/// Testes da SqlConnectionFactory. Usa o banco master da LocalDB, que sempre existe, então nenhum banco é criado ou apagado.
/// </summary>
[Trait("Categoria", "Integracao")]
public sealed class SqlConnectionFactoryTests
{
    private const string ConnectionStringMaster =
        @"Server=(localdb)\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

    /// <summary>
    /// A fábrica deve recusar connection string vazia logo na criação,
    /// em vez de falhar só na primeira tentativa de conexão.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_ConnectionStringVazia_LancaExcecao(string connectionString)
    {
        Assert.Throws<ArgumentException>(() => new SqlConnectionFactory(connectionString));
    }

    /// <summary>
    /// O AbrirAsync deve devolver a conexão já aberta, pronta para uso pelos repositórios.
    /// </summary>
    [Fact]
    public async Task AbrirAsync_LocalDb_DevolveConexaoAberta()
    {
        var fabrica = new SqlConnectionFactory(ConnectionStringMaster);

        // await using: fecha a conexão no fim do teste
        await using var conexao = await fabrica.AbrirAsync(TestContext.Current.CancellationToken);

        Assert.Equal(ConnectionState.Open, conexao.State);
    }
}