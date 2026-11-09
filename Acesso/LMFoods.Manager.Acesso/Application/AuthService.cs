using LMFoods.Manager.Acesso.Abstractions;
using LMFoods.Manager.Acesso.Contracts;
using LMFoods.Manager.Acesso.Domain;

namespace LMFoods.Manager.Acesso.Application;

/// <summary>
/// Login em duas etapas (login e PIN) e registro das sessões.
/// </summary>
/// <param name="usuarios">busca e salva os usuários</param>
/// <param name="sessoes">abre e fecha as sessões</param>
/// <param name="pinHasher">confere o PIN com o hash</param>
/// <param name="politica">quantas tentativas e quanto tempo de bloqueio</param>
/// <param name="relogio">hora atual (nos testes dá para usar uma hora falsa)</param>
// As dependências chegam pelo construtor (quem cria a classe é a injeção de dependência)
internal sealed class AuthService(IUsuarioRepository usuarios, ISessaoUsuarioRepository sessoes,
                                  IPinHasher pinHasher, PoliticaBloqueio politica,
                                  TimeProvider relogio) : IAuthService
{
    /// <summary>
    /// busca o usuário pelo login para a tela do PIN.
    /// Retorna null para login vazio, inexistente ou de usuário inativo.
    /// </summary>
    public async Task<UsuarioIdentificadoDto?> IdentificarAsync(string login, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(login))
            return null;

        // Procura o usuário pelo login
        var usuario = await usuarios.ObterPorLoginAsync(login.Trim(), ct);

        // Não achou ou está inativo: devolve null nos dois casos, para não mostrar qual foi
        if (usuario is null || !PodeEntrar(usuario))
            return null;

        // Manda para a tela só o id, o nome e o cargo (o PIN fica aqui dentro)
        return new UsuarioIdentificadoDto(usuario.Id, usuario.Nome, usuario.Cargo);
    }


    /// <summary>
    /// Confere o PIN, controla tentativas e bloqueio e, se aceito, abre a sessão.
    /// </summary>
    public async Task<ResultadoLogin> AutenticarAsync(LoginRequest request, CancellationToken ct = default)
    {
        // Se a tela mandar nada, dá erro na hora
        ArgumentNullException.ThrowIfNull(request);

        // Busca o usuário de novo pelo id, para pegar os dados mais atuais do banco
        var usuario = await usuarios.ObterPorIdAsync(request.IdUsuario, ct);

        // Sumiu ou foi desativado entre uma tela e outra: não deixa entrar
        if (usuario is null || !PodeEntrar(usuario))
            return ResultadoLogin.Inativo();

        // Pega a hora atual em UTC
        var agora = relogio.GetUtcNow().UtcDateTime;

        // Se está bloqueado, recusa sem nem olhar o PIN
        if (usuario.EstaBloqueado(agora))
            return ResultadoLogin.Bloqueado(ParaUtc(usuario.BloqueadoAte!.Value));

        // PIN errado: conta a tentativa
        if (!pinHasher.Verificar(request.Pin, usuario.PinHash))
            return await RegistrarPinIncorretoAsync(usuario, agora, ct);

        // PIN certo: termina o login
        return await ConcluirLoginAsync(usuario, ct);
    }


    /// <summary>
    /// Registra o logout. Encerrar uma sessão já encerrada não tem efeito.
    /// </summary>
    public async Task EncerrarSessaoAsync(int idSessao, CancellationToken ct = default)
    {
        // Grava a hora de saída da sessão no banco
        await sessoes.EncerrarAsync(idSessao, ct);
    }


    /// <summary>
    /// Soma a tentativa errada, grava no banco e informa se ainda há tentativas ou se bloqueou.
    /// </summary>
    private async Task<ResultadoLogin> RegistrarPinIncorretoAsync(Usuario usuario, DateTime agora, CancellationToken ct)
    {
        usuario.RegistrarPinIncorreto(agora, politica);
        await usuarios.SalvarTentativasAsync(usuario, ct);

        // Se bloqueou agora, avisa até quando; se não, avisa quantas tentativas faltam
        return usuario.EstaBloqueado(agora)
            ? ResultadoLogin.Bloqueado(ParaUtc(usuario.BloqueadoAte!.Value))
            : ResultadoLogin.PinIncorreto(usuario.TentativasRestantes(politica));
    }


    /// <summary>
    /// PIN correto: limpa as tentativas (se houver), abre a sessão e monta o usuário logado.
    /// </summary>
    private async Task<ResultadoLogin> ConcluirLoginAsync(Usuario usuario, CancellationToken ct)
    {
        // Só grava no banco se havia algo para limpar
        if (usuario.TentativasFalhas > 0 || usuario.BloqueadoAte is not null)
        {
            usuario.RegistrarLoginComSucesso();
            await usuarios.SalvarTentativasAsync(usuario, ct);
        }

        // Abre a sessão no banco e guarda o id dela (o caixa pode ser null)
        var idSessao = await sessoes.AbrirAsync(usuario.Id, usuario.Caixa?.Id, ct);

        // Monta o usuário logado que vai para a tela
        return ResultadoLogin.Ok(new UsuarioLogadoDto(
            usuario.Id, usuario.Nome, usuario.Cargo,
            usuario.Filial.Id, usuario.Filial.Nome,
            usuario.Caixa?.Id,    // null se o usuário não tem caixa
            usuario.Caixa?.Nome,  // null se o usuário não tem caixa
            idSessao));           // usado depois para fazer o logout
    }


    /// <summary>
    /// Só entra se o usuário e a filial dele estiverem ativos
    /// </summary>    
    private static bool PodeEntrar(Usuario usuario) => usuario.Ativo && usuario.Filial.Ativo;


    /// <summary>
    /// O banco devolve as datas sem fuso (DATETIME2). Diz que data é UTC para a tela converter certo para o horário de Brasília
    /// </summary>   
    private static DateTimeOffset ParaUtc(DateTime data) =>
        new(DateTime.SpecifyKind(data, DateTimeKind.Utc));
}