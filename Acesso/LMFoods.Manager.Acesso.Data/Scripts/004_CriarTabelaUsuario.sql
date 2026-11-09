CREATE TABLE acesso.Usuario 
(
    id INT IDENTITY (1, 1) NOT NULL,
    filial_id INT NOT NULL,
    caixa_id INT NULL, -- NULL para quem não trabalha em caixa (gerente, admin)
    nome NVARCHAR (100) NOT NULL,
    cargo NVARCHAR (50) NULL, -- só exibição; permissões entram com perfil_id na V3
    login NVARCHAR (50) NOT NULL,
    pin_hash VARCHAR(100) NOT NULL, -- BCrypt do PIN de 4 dígitos
    ativo BIT NOT NULL CONSTRAINT DF_Usuario_ativo DEFAULT 1,
    motivo_inativacao NVARCHAR (200) NULL,
    tentativas_falhas INT NOT NULL CONSTRAINT DF_Usuario_tentativas_falhas DEFAULT 0,
    bloqueado_ate DATETIME2 (0) NULL, -- UTC
    -- Preenchido pelo banco com a hora atual em UTC, sem frações de segundos
    data_cadastro DATETIME2 (0) NOT NULL CONSTRAINT DF_Usuario_data_cadastro DEFAULT SYSUTCDATETIME (),
    versao ROWVERSION NOT NULL, -- controle de concorrência na edição
    CONSTRAINT PK_Usuario PRIMARY KEY (id),
    CONSTRAINT UQ_Usuario_login UNIQUE (login),
    CONSTRAINT FK_Usuario_Filial FOREIGN KEY (filial_id) REFERENCES acesso.Filial (id),
    -- O caixa precisa ser da mesma filial do usuário (ignorada quando caixa_id é NULL)
    CONSTRAINT FK_Usuario_Caixa FOREIGN KEY (caixa_id, filial_id) REFERENCES acesso.Caixa (id, filial_id),
    -- Usuário inativo precisa ter o motivo da inativação preenchido
    CONSTRAINT CK_Usuario_motivo_inativacao CHECK (
        ativo = 1  OR motivo_inativacao IS NOT NULL
    ),
    -- O contador de PINs errados nunca pode ficar negativo
    CONSTRAINT CK_Usuario_tentativas_falhas CHECK (tentativas_falhas >= 0)
);

CREATE INDEX IX_Usuario_filial_id ON acesso.Usuario (filial_id); -- indexacao (busca direto a linha correta)
CREATE INDEX IX_Usuario_caixa_id ON acesso.Usuario (caixa_id);   -- indexacao (busca direto a linha correta)