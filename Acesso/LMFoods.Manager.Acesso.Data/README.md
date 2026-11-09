# LMFoods.Manager.Acesso.Data

Camada de acesso a dados do módulo **Acesso** (login, usuários, filiais e caixas).
Esta pasta guarda os scripts SQL que criam o banco e os dados de teste.

## Estrutura

```
├── LMFoods.Manager.Acesso.Data/
│   ├── README.md
│   ├── Scripts/
│   │   ├── 001_CriarSchemaAcesso.sql
│   │   ├── 002_CriarTabelaFilial.sql
│   │   ├── 003_CriarTabelaCaixa.sql
│   │   ├── 004_CriarTabelaUsuario.sql
│   │   └── 005_CriarTabelaSessaoUsuario.sql
│   └── Seeds/
        └── 001_DadosTeste.sql
```

## Tabelas

Todas as tabelas ficam no schema `acesso`.

| Tabela          | O que guarda                                                  |
| --------------- | ------------------------------------------------------------- |
| `Filial`        | Cada unidade da LM Food                                       |
| `Caixa`         | Pontos de venda (balcões) de cada filial                      |
| `Usuario`       | Pessoas que usam o sistema. Login digitado + PIN de 4 dígitos |
| `SessaoUsuario` | Cada login e logout. Uma linha por sessão                     |

### Regras importantes

- O caixa de um usuário precisa ser da mesma filial que ele (FK composta `FK_Usuario_Caixa`).
- Usuário sem caixa (gerente, administrador) fica com `caixa_id` = `NULL`.
- Usuário inativo precisa ter `motivo_inativacao` preenchido.
- O PIN é guardado como hash BCrypt em `pin_hash`, nunca em texto puro.
- `tentativas_falhas` e `bloqueado_ate` controlam o bloqueio após PINs errados.
- Filiais, caixas e usuários são desativados (`ativo = 0`), nunca apagados.

## Convenções

| Item        | Padrão                                                                                       |
| ----------- | -------------------------------------------------------------------------------------------- |
| Tabelas     | PascalCase, singular (`SessaoUsuario`)                                                       |
| Colunas     | snake_case (`filial_id`, `data_cadastro`)                                                    |
| Datas       | `DATETIME2(0)` em UTC (`SYSUTCDATETIME()`). A conversão para o horário local é feita na tela |
| Textos      | `NVARCHAR`, inseridos com `N'...'`. Exceção: `pin_hash` é `VARCHAR`                          |
| Comentários | `--` para uma linha, `/* */` para várias linhas                                              |

As regras (constraints) sempre têm nome, com prefixo pelo tipo:

| Prefixo | Tipo              |
| ------- | ----------------- |
| `PK_`   | Chave primária    |
| `FK_`   | Chave estrangeira |
| `UQ_`   | Valor único       |
| `CK_`   | Validação         |
| `DF_`   | Valor padrão      |
| `IX_`   | Índice            |

Sem nome, o SQL Server gera um nome aleatório diferente em cada banco, e scripts futuros não conseguem alterar a regra.

## Como os scripts rodam

O app vai rodar os scripts automaticamente ao abrir, usando o DbUp (`DatabaseMigrator`, em desenvolvimento).
O DbUp grava na tabela `dbo.SchemaVersions` quais scripts já foram executados, então cada script roda uma única vez.

Os seeds só rodam quando a configuração `IncluirDadosTeste` é `true` (Development e Teste).

## Rodar manualmente pelo terminal

Útil para validar os scripts sem abrir o app. Requer a LocalDB e o `sqlcmd` (`winget install sqlcmd`).

> ⚠️ Use sempre um banco separado, como `LMFoods_Banco_Validacao`. Não rode à mão no `LMFoods_Banco`, que é controlado pelo DbUp: as tabelas criadas manualmente não ficam registradas, e o app tentaria criá-las de novo.

Na pasta `LMFoods.Manager.Acesso.Data`:

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE LMFoods_Banco_Validacao"

sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -b -f 65001 -i Scripts\001_CriarSchemaAcesso.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -b -f 65001 -i Scripts\002_CriarTabelaFilial.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -b -f 65001 -i Scripts\003_CriarTabelaCaixa.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -b -f 65001 -i Scripts\004_CriarTabelaUsuario.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -b -f 65001 -i Scripts\005_CriarTabelaSessaoUsuario.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -b -f 65001 -i Seeds\001_DadosTeste.sql
```

- `-b` para no primeiro erro e mostra o arquivo e a linha.
- `-f 65001` lê os arquivos como UTF-8, para os acentos do seed não virarem caracteres estranhos.

Conferir os dados:

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -Q "SELECT login, nome, cargo, ativo FROM acesso.Usuario"
```

Apagar o banco de validação no final:

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DROP DATABASE LMFoods_Banco_Validacao"
```

Conferir tabela Usuarios:

```
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LMFoods_Banco_Validacao -Q "SELECT login, nome, cargo, ativo FROM acesso.Usuario"
```

## Usuários de teste

Criados pelo `Seeds/001_DadosTeste.sql`. PIN de todos: **1234**.

| Login       | Situação                           |
| ----------- | ---------------------------------- |
| `ana`       | Gerente Geral, sem caixa           |
| `joao`      | Operador de caixa da Filial Centro |
| `inativo`   | Usuário desativado                 |
| `bloqueado` | Usuário bloqueado por tentativas   |

## Como alterar o banco

1. **Nunca edite um script que já foi executado.** O DbUp não roda de novo um script já registrado, então a alteração nunca chegaria aos bancos existentes.
2. Crie um novo arquivo com o próximo número, por exemplo `Scripts/006_AdicionarTelefoneUsuario.sql`, com o `ALTER TABLE` necessário.
3. Dados de teste novos vão em `Seeds/`, também com o próximo número.
