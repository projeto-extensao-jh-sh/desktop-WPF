CREATE TABLE acesso.Caixa 
(
    id INT IDENTITY (1, 1) NOT NULL,
    filial_id INT NOT NULL,
    nome NVARCHAR (50) NOT NULL,
    ativo BIT NOT NULL CONSTRAINT DF_Caixa_ativo DEFAULT 1, -- caixa fechadO é desativada, nunca apagadO
    -- Preenchido pelo banco com a hora atual em UTC, sem frações de segundos
    data_cadastro DATETIME2 (0) NOT NULL CONSTRAINT DF_Caixa_data_cadastro DEFAULT SYSUTCDATETIME (),
    CONSTRAINT PK_Caixa PRIMARY KEY (id),
    CONSTRAINT UQ_Caixa_filial_nome UNIQUE (filial_id, nome),
    -- Permite que Usuario garanta que o caixa é da mesma filial (FK composta)
    -- Necessária para a FK composta em Usuario (garante que o caixa é da mesma filial do usuário)
    CONSTRAINT UQ_Caixa_id_filial UNIQUE (id, filial_id),
    CONSTRAINT FK_Caixa_Filial FOREIGN KEY (filial_id) REFERENCES acesso.Filial (id)
);