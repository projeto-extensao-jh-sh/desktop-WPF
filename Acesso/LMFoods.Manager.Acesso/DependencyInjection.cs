using LMFoods.Manager.Acesso.Abstractions;
using LMFoods.Manager.Acesso.Application;
using LMFoods.Manager.Acesso.Contracts;
using LMFoods.Manager.Acesso.Domain;
using LMFoods.Manager.Acesso.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LMFoods.Manager.Acesso;

/// <summary>
/// Registra as regras de negócio do módulo Acesso na injeção de dependência.
/// Os repositórios não ficam aqui: eles são registrados pelo AddAcessoData (Acesso.Data).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAcesso(this IServiceCollection services)
    {
        // Serviço do login que a tela vai usar
        services.AddTransient<IAuthService, AuthService>();

        // Hash do PIN com BCrypt (não guarda nada, então uma instância serve para o app todo)
        services.AddSingleton<IPinHasher, BCryptPinHasher>();

        // Regra de bloqueio: 5 tentativas e 5 minutos
        services.AddSingleton(PoliticaBloqueio.Padrao);

        // Relógio do sistema. O TryAdd só registra se ninguém registrou antes
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}