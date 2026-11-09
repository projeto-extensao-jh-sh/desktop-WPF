namespace LMFoods.Manager.Shared.Data.Tests.Unit;

/// <summary>
/// Validações de entrada do migrador. Não acessam o banco: o erro acontece antes da conexão.
/// </summary>
[Trait("Categoria", "Unitario")]
public sealed class DatabaseMigratorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Migrar_ConnectionStringVazia_LancaExcecao(string connectionString)
    {
        Assert.Throws<ArgumentException>(() =>
            DatabaseMigrator.Migrar(connectionString, incluirDadosTeste: false, typeof(DatabaseMigratorTests).Assembly));
    }

    [Fact]
    public void Migrar_SemAssemblies_LancaExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            DatabaseMigrator.Migrar("Server=x;Database=y;", incluirDadosTeste: false));
    }
}