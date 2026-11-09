namespace LMFoods.Manager.Shared.Data;

/// <summary>
/// Configurações da camada de dados, lidas da seção "Data" do appsettings.json em LMFoods.Manager.Wpf.
/// </summary>
public sealed class DataOptions
{
    // nome da sessao no json
    public const string Secao = "Data";

    /// <summary>De onde vêm os dados: Mock (memória) ou Sql (SQL Server)</summary>
    public ProvedorDados Provedor { get; set; } = ProvedorDados.Mock;

    /// <summary>Obrigatória quando o provedor é Sql</summary>
    public string? ConnectionString { get; set; }

    /// <summary>Executa os seeds (usuários de teste). Nunca ligar em produção</summary>
    public bool IncluirDadosTeste { get; set; }

    /// <summary> Confere se a configuração é válida. Falha na abertura do app, em vez de falhar no primeiro login </summary>
    public void Validar()
    {
        if (Provedor == ProvedorDados.Sql && string.IsNullOrWhiteSpace(ConnectionString))
            throw new InvalidOperationException(
                $"A configuração '{Secao}:ConnectionString' é obrigatória quando o provedor é Sql.");
    }
}


/// <summary>De onde vêm os dados: Mock (memória) ou Sql (SQL Server)</summary>
public enum ProvedorDados
{
    Mock,
    Sql
}