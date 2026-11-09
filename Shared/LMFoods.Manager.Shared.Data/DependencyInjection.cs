using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMFoods.Manager.Shared.Data;

/// <summary>
/// Registra a infraestrutura de banco compartilhada na injeção de dependência.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Lê e valida a seção "Data" do appsettings.json e registra o DataOptions.
    /// Com o provedor Sql, registra também a SqlConnectionFactory.
    /// </summary>
    public static IServiceCollection AddSharedData(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // Sem a seção no appsettings.json, usa os valores padrão (Mock, sem dados de teste)
        var opcoes = configuration.GetSection(DataOptions.Secao).Get<DataOptions>() ?? new DataOptions();

        // Falha na abertura do app, e não no primeiro login
        opcoes.Validar();

        // Um conexão de banco é compartilhada entre todos os repositórios, então registra como singleton
        services.AddSingleton(opcoes);

        // Com o provedor Mock não existe banco, então não há conexão para criar
        if (opcoes.Provedor == ProvedorDados.Sql)
            services.AddSingleton(new SqlConnectionFactory(opcoes.ConnectionString!));

        return services;
    }
}