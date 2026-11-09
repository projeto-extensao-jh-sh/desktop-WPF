namespace LMFoods.Manager.Acesso.Domain;

/// <summary>
/// Pessoa que usa o sistema. Tabela acesso.Usuario.
/// Guarda as regras de tentativas e bloqueio do PIN.
/// </summary>
internal sealed class Usuario
{
    // Campos alterados apenas pelos métodos de regra (RegistrarPinIncorreto, RegistrarLoginComSucesso)
    private int _tentativasFalhas;
    private DateTime? _bloqueadoAte;

    public required int Id { get; init; }
    public required string Login { get; init; }
    public required string Nome { get; init; }
    public string? Cargo { get; init; }
    public required string PinHash { get; init; }
    public required Filial Filial { get; init; }

    /// <summary>NULL para quem não trabalha em caixa (gerente, administrador).</summary>
    public Caixa? Caixa { get; init; }

    public bool Ativo { get; init; } = true;
    public string? MotivoInativacao { get; init; }

    public int TentativasFalhas
    {
        get => _tentativasFalhas;
        init => _tentativasFalhas = value;
    }

    /// <summary>Data e hora em UTC até quando o usuário fica bloqueado.</summary>
    public DateTime? BloqueadoAte
    {
        get => _bloqueadoAte;
        init => _bloqueadoAte = value;
    }

    /// <summary>
    /// Verifica se o usuário ainda está bloqueado no momento informado.
    /// Retorna false quando não há bloqueio (BloqueadoAte nulo) ou quando ele já expirou.
    /// </summary>
    public bool EstaBloqueado(DateTime agoraUtc) => _bloqueadoAte > agoraUtc;

    /// <summary>
    /// Quantas tentativas erradas ainda restam antes do bloqueio. Usado na mensagem da tela do PIN.
    /// Nunca retorna negativo, mesmo se a política for alterada para um limite menor.
    /// </summary>
    public int TentativasRestantes(PoliticaBloqueio politica) =>
        Math.Max(0, politica.MaxTentativas - _tentativasFalhas);

    /// <summary>
    /// Soma uma tentativa errada. Ao atingir o limite, bloqueia o usuário e zera o contador.
    /// </summary>
    public void RegistrarPinIncorreto(DateTime agoraUtc, PoliticaBloqueio politica)
    {
        _tentativasFalhas++;

        if (_tentativasFalhas >= politica.MaxTentativas)
        {
            _bloqueadoAte = agoraUtc + politica.DuracaoBloqueio;
            _tentativasFalhas = 0;
        }
    }

    /// <summary>
    /// Login aceito: limpa as tentativas e o bloqueio.
    /// </summary>
    public void RegistrarLoginComSucesso()
    {
        _tentativasFalhas = 0;
        _bloqueadoAte = null;
    }
}
