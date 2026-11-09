namespace LMFoods.Manager.Acesso.Contracts;

/// <summary>Motivo pelo qual o login foi recusado</summary>
public enum MotivoFalhaLogin
{
    /// <summary>PIN incorreto.</summary>
    PinIncorreto = 1,

    /// <summary>Usuário não encontrado ou desativado.</summary>
    UsuarioInativo = 2,

    /// <summary>Usuário bloqueado temporariamente por excesso de tentativas.</summary>
    UsuarioBloqueado = 3
}