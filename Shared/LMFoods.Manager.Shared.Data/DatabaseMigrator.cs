using DbUp;
using System.Reflection;

namespace LMFoods.Manager.Shared.Data;

/// <summary>
/// Cria o banco (se não existir) e executa os scripts do módulo Acesso que ainda não rodaram.
/// </summary>
public static class DatabaseMigrator
{
    /// <param name="assembliesComScripts">
    /// DLLs dos módulos que contêm scripts embutidos, na ordem em que devem rodar.
    /// </param>
    public static void Migrar(string connectionString, bool incluirDadosTeste, params Assembly[] assembliesComScripts)
    {
        // Se vier vazia para na hora e retorna erro
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        if (assembliesComScripts.Length == 0)
            throw new ArgumentException("Informe ao menos uma DLL com scripts.", nameof(assembliesComScripts));


        // Cria o banco se não existir
        EnsureDatabase.For.SqlDatabase(connectionString);

        // Um módulo por vez, na ordem informada (ex.: Acesso antes de Produtos)
        foreach (var assembly in assembliesComScripts)
        {
            var upgrader = DeployChanges.To.SqlDatabase(connectionString)
                            .WithScriptsEmbeddedInAssembly(assembly,
                                nome => DeveExecutar(nome, incluirDadosTeste))
                            // Se um script falhar no meio, tudo o que ele fez é desfeito
                            .WithTransactionPerScript()
                            // O DbUp trata $texto$ como variável, e o hash BCrypt do seed ($2a$11$...) seria corrompido
                            .WithVariablesDisabled()
                            // Mensagens aparecem na janela Output do Visual Studio
                            .LogToTrace()
                            .Build();

            // executa: compara os scripts com a tabela dbo.SchemaVersions, roda só os que faltam
            var resultado = upgrader.PerformUpgrade();

            if (!resultado.Successful)
                throw new InvalidOperationException(
                    "Falha ao atualizar o banco do módulo Acesso. Veja a exceção interna.", resultado.Error);
        }
    }


    /// <summary>
    /// Define se cada script deve rodar (SIM/NAO)
    /// </summary>
    /// <param name="nomeRecurso"></param>
    /// <param name="incluirDadosTeste"></param>
    /// <returns></returns>
    private static bool DeveExecutar(string nomeRecurso, bool incluirDadosTeste)
    {
        // Scripts de estrutura: sempre rodam
        if (nomeRecurso.Contains(".Scripts."))
            return true;

        // Seeds: só rodam se os dados de teste estiverem ligados
        if (nomeRecurso.Contains(".Seeds.") && incluirDadosTeste)
            return true;

        // Qualquer outro arquivo: não roda
        return false;
    }
}