namespace LMFoods.Manager.Shared.Data.Tests.Unit;

[Trait("Categoria", "Unitario")]
public sealed class DataOptionsTests
{
    /// <summary>
    /// Sem configuração, o app não pode usar o banco nem criar usuários de teste.
    /// </summary>
    [Fact]
    public void ValoresPadrao_SaoOsSeguros()
    {
        var opcoes = new DataOptions();

        Assert.Equal(ProvedorDados.Mock, opcoes.Provedor);
        Assert.False(opcoes.IncluirDadosTeste);
    }

    [Fact]
    public void Validar_SqlSemConnectionString_LancaExcecao()
    {
        var opcoes = new DataOptions { Provedor = ProvedorDados.Sql };

        Assert.Throws<InvalidOperationException>(opcoes.Validar);
    }

    [Fact]
    public void Validar_SqlComConnectionString_NaoLancaExcecao()
    {
        var opcoes = new DataOptions { Provedor = ProvedorDados.Sql, ConnectionString = "Server=x;Database=y;" };

        opcoes.Validar();
    }

    [Fact]
    public void Validar_MockSemConnectionString_NaoLancaExcecao()
    {
        var opcoes = new DataOptions { Provedor = ProvedorDados.Mock };

        opcoes.Validar();
    }
}