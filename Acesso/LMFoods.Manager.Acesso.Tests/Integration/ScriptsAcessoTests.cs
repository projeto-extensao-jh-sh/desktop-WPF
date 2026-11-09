using LMFoods.Manager.Acesso.Data;
using LMFoods.Manager.Shared.Data;
using Microsoft.Data.SqlClient;

namespace LMFoods.Manager.Acesso.Tests.Integration;

/// <summary>
/// Valida os scripts SQL do módulo Acesso executados pelo migrador
/// Cada teste usa um banco novo na LocalDB, apagado no final
/// </summary>
[Trait("Categoria", "Integracao")]
public sealed class ScriptsAcessoTests : IDisposable
{
    private const string Servidor = @"(localdb)\MSSQLLocalDB";
    private readonly string _banco = $"LMFoods_Teste_{Guid.NewGuid():N}";
    private readonly string _connectionString;

    // Roda antes de cada teste
    public ScriptsAcessoTests() => _connectionString = MontarConnectionString(_banco);


    // Modelo Tests - Método_Cenário_ResultadoEsperado
    /// <summary>
    /// Simula a primeira abertura do app em desenvolvimento ou teste:
    /// o banco não existe, e o migrador deve criar a estrutura e inserir os dados de teste.
    /// </summary>
    [Fact]
    public void Migrar_ComDadosTeste_CriaTabelasEUsuarios()
    {
        Migrar(incluirDadosTeste: true);

        Assert.Equal(4, Contar("SELECT COUNT(*) FROM sys.tables WHERE schema_id = SCHEMA_ID('acesso')"));
        Assert.True(Contar("SELECT COUNT(*) FROM acesso.Usuario") > 0);
    }


    /// <summary>
    /// No cliente, os usuários de teste não podem ir para produção.
    /// Garante que o filtro do migrador ignora a pasta Seeds.
    /// </summary>
    [Fact]
    public void Migrar_SemDadosTeste_NaoInsereUsuarios()
    {
        Migrar(incluirDadosTeste: false);

        Assert.Equal(0, Contar("SELECT COUNT(*) FROM acesso.Usuario"));
    }


    /// <summary>
    /// Garante que o app pode ser aberto quantas vezes for preciso sem estragar o banco.
    /// </summary>
    [Fact]
    public void Migrar_ExecutadoDuasVezes_NaoRepeteScripts()
    {
        Migrar(incluirDadosTeste: true);
        var usuariosAntes = Contar("SELECT COUNT(*) FROM acesso.Usuario");

        Migrar(incluirDadosTeste: true);

        Assert.Equal(usuariosAntes, Contar("SELECT COUNT(*) FROM acesso.Usuario"));
    }


    /// <summary>
    /// Atalho para rodar o migrador com os scripts do módulo Acesso no banco deste teste.
    /// </summary>
    private void Migrar(bool incluirDadosTeste) =>
        DatabaseMigrator.Migrar(_connectionString, incluirDadosTeste, AcessoDataAssembly.Assembly);


    /// <summary>
    /// Roda uma consulta COUNT no banco de teste e devolve o número retornado.
    /// </summary>    
    private int Contar(string sql)
    {
        using var conexao = new SqlConnection(_connectionString);
        conexao.Open();
        using var comando = new SqlCommand(sql, conexao);
        return (int)comando.ExecuteScalar()!;
    }


    /// <summary>
    /// Monta a connection string para qualquer banco
    /// </summary>    
    private static string MontarConnectionString(string banco) =>
        $"Server={Servidor};Database={banco};Trusted_Connection=True;TrustServerCertificate=True;";


    /// <summary>
    /// Apaga o banco de teste. O xUnit chama este método ao final de cada teste.
    /// </summary>
    public void Dispose()
    {
        // Fecha as conexões abertas pelo pool, senão o banco não pode ser apagado
        SqlConnection.ClearAllPools();

        using var conexao = new SqlConnection(MontarConnectionString("master"));
        conexao.Open();
        using var comando = new SqlCommand(
            $"""
            IF DB_ID('{_banco}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_banco}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_banco}];
            END
            """, conexao);
        comando.ExecuteNonQuery();
    }
}