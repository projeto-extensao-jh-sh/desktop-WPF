-- Dados de teste. Rodam só em Development/Teste (IncluirDadosTeste = true).
-- PIN de todos os usuários: 1234

SET IDENTITY_INSERT acesso.Filial ON;
-- Cria filiais para teste
INSERT INTO acesso.Filial (id, nome) VALUES
    (1, N'Maderera Bozovich'),
    (2, N'Metalurgia Maccaferri'),
    (3, N'Engenharia de cartões'),
    (4, N'Papeis YciP');
SET IDENTITY_INSERT acesso.Filial OFF;

SET IDENTITY_INSERT acesso.Caixa ON;
-- Cria caixas para teste
INSERT INTO acesso.Caixa (id, filial_id, nome) VALUES
    (1, 1, N'Caixa 01'),
    (2, 1, N'Caixa 02'),
    (3, 2, N'Caixa 01');
SET IDENTITY_INSERT acesso.Caixa OFF;

-- Hash do PIN 1234. Copiado na constante HashDoSeed de Acesso.Tests/Unit/BCryptPinHasherTests.cs: se mudar aqui, mude lá.
DECLARE @pin VARCHAR(100) = '$2a$11$lygi.wgZKCkygMiwyM1KXO/WjMwphJgBsqnyAN9Gy7oIFPFXgIrCO';

-- Cria usuarios para teste
INSERT INTO acesso.Usuario
    (filial_id, caixa_id, nome, cargo, login, pin_hash, ativo, motivo_inativacao, bloqueado_ate)
VALUES
    (1, NULL, N'Ana Carvalho',      N'Gerente Geral',     N'ana',       @pin, 1, NULL,                    NULL),
    (2, 3,    N'João Operador',     N'Operador de Caixa', N'joao',      @pin, 1, NULL,                    NULL),
    (1, 1,    N'Usuário Inativo',   N'Operador de Caixa', N'inativo',   @pin, 0, N'Desligado da empresa', NULL),
    (1, 2,    N'Usuário Bloqueado', N'Operador de Caixa', N'bloqueado', @pin, 1, NULL,                    DATEADD(YEAR, 10, SYSUTCDATETIME()));