using Microsoft.Data.SqlClient;

namespace LMFoods.Manager.Shared.Data;

/// <summary>
/// Cria conexões abertas com o SQL Server. Os repositórios recebem esta classe em vez da connection string, que fica num lugar só.
/// </summary>
public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        // Recusa connection string vazia logo na criação
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }


    /// <summary>
    /// Cria e abre uma conexão. Quem chama é responsável por fechá-la (await using).
    /// </summary>
    public async Task<SqlConnection> AbrirAsync(CancellationToken ct = default)
    {
        var conexao = new SqlConnection(_connectionString);
        try
        {
            await conexao.OpenAsync(ct);
            return conexao;
        }
        catch
        {
            // Se não conseguiu abrir, libera o objeto antes de repassar o erro
            await conexao.DisposeAsync();
            throw;
        }
    }
}