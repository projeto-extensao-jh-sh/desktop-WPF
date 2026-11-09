CREATE TABLE acesso.Filial 
(
    id INT IDENTITY (1, 1) NOT NULL,
    nome NVARCHAR (100) NOT NULL,
    ativo  BIT  NOT NULL CONSTRAINT DF_Filial_ativo DEFAULT 1,   -- filial fechada é desativada, nunca apagada
    -- Preenchido pelo banco com a hora atual em UTC, sem frações de segundos
    data_cadastro DATETIME2 (0) NOT NULL CONSTRAINT DF_Filial_data_cadastro DEFAULT SYSUTCDATETIME (),
    CONSTRAINT PK_Filial PRIMARY KEY (id),
    CONSTRAINT UQ_Filial_nome UNIQUE (nome)   -- valor unico
);