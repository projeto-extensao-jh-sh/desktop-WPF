using System.Reflection;

namespace LMFoods.Manager.Acesso.Data;

/// <summary>
/// Referência à DLL deste projeto, onde estão os scripts SQL embutidos.
/// Usada para entregar os scripts do módulo Acesso ao DatabaseMigrator.
/// É necessário ter uma classe pública para que o DbUp em LMFoods.Manager.Shared.Data/DatabaseMigrator.cs consiga localizar a DLL.
/// </summary>
public static class AcessoDataAssembly
{
    public static Assembly Assembly { get; } = typeof(AcessoDataAssembly).Assembly;
}