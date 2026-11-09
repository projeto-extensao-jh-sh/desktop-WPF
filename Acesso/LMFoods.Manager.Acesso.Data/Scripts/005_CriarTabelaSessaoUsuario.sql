CREATE TABLE acesso.SessaoUsuario 
(
    id INT IDENTITY (1, 1) NOT NULL,
    usuario_id INT NOT NULL,
    caixa_id INT NULL, -- NULL quando o usuário não tem caixa    
    -- Preenchido pelo banco com a hora atual em UTC, sem frações de segundos
    data_hora_login DATETIME2(0) NOT NULL CONSTRAINT DF_SessaoUsuario_data_hora_login DEFAULT SYSUTCDATETIME (),
    data_hora_logout DATETIME2(0) NULL, -- NULL enquanto a sessão está aberta
    CONSTRAINT PK_SessaoUsuario PRIMARY KEY (id),
    CONSTRAINT FK_SessaoUsuario_Usuario FOREIGN KEY (usuario_id) REFERENCES acesso.Usuario (id),
    CONSTRAINT FK_SessaoUsuario_Caixa FOREIGN KEY (caixa_id) REFERENCES acesso.Caixa (id),
    -- O logout não pode ser anterior ao login (so salva a linha se condicao for verdaeira)
    CONSTRAINT CK_SessaoUsuario_logout CHECK (
        data_hora_logout IS NULL OR data_hora_logout >= data_hora_login
    )
);

CREATE INDEX IX_SessaoUsuario_usuario_id ON acesso.SessaoUsuario (usuario_id, data_hora_login);
CREATE INDEX IX_SessaoUsuario_caixa_id ON acesso.SessaoUsuario (caixa_id); -- indexacao (busca direto a linha correta)