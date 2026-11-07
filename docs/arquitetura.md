# Arquitetura do sistema

Este documento explica como o código do LM Foods Manager é organizado e por quê.

## Visão geral

O sistema é um **monólito modular**: um único aplicativo, dividido em **módulos por assunto de negócio** (acesso, produtos, vendas, relatórios). Cada módulo tem suas próprias regras e suas próprias tabelas no banco.

Começar como um monólito deixa o desenvolvimento e os testes mais simples. Separar por módulos desde o início permite, no futuro, transformar um módulo em uma API sem reescrever as regras.

```
Aplicativo WPF (telas)
   │
   ├── Módulo Acesso      → login, usuários, filiais e caixas
   ├── Módulo Produtos    → (próxima versão)
   ├── Módulo Vendas      → (próxima versão)
   │
   └── Shared             → código técnico usado por todos os módulos
```

## Divisão de cada módulo

Cada módulo é dividido em projetos com responsabilidades separadas:

| Projeto | O que contém |
|---|---|
| **Contracts** | O que o módulo oferece para as telas: interfaces dos serviços e os dados que vão e voltam (DTOs). É o único projeto do módulo que as telas conhecem |
| **Regras de negócio** | Entidades, validações e os serviços que implementam os contratos. Não sabe qual banco de dados está sendo usado |
| **Data** | Acesso ao banco: repositórios, scripts SQL e dados de teste |
| **Tests** | Testes automatizados do módulo |

O projeto **Shared** guarda apenas código técnico comum a todos os módulos, como a conexão com o banco e a execução dos scripts SQL. Ele não tem regras de negócio e não conhece nenhum módulo.

## Direção das dependências

```
Telas (WPF) ──► Contracts ◄── Regras de negócio ◄── Data ──► Shared
```

- As **telas** usam apenas os contratos. Nunca acessam o banco diretamente.
- As **regras de negócio** não dependem do banco. Elas definem interfaces (repositórios), e o projeto **Data** as implementa.
- O **Shared** não depende de nenhum módulo.

Com essa direção, dá para trocar o banco de dados, ou colocar uma API no meio, sem mudar as telas nem as regras.

## Padrões usados

- **Repository:** cada acesso ao banco fica numa classe de repositório, escondida atrás de uma interface. As regras só conhecem a interface.
- **Injeção de dependência:** as classes recebem o que precisam pelo construtor. Quem monta tudo é o aplicativo, na inicialização.
- **Scripts versionados:** o banco é criado e atualizado por scripts SQL numerados, que ficam no repositório e rodam automaticamente ao abrir o aplicativo.

## Fases da origem dos dados

O sistema foi preparado para mudar de onde vêm os dados só alterando a configuração (`appsettings.json`), sem mudar o código das telas e das regras:

| Fase | Origem dos dados | Uso |
|---|---|---|
| 1 | **Mock** (dados em memória) | Desenvolvimento e testes das telas |
| 2 | **SQL Server local** (LocalDB) | Testes com banco real, em um computador |
| 3 | **Azure SQL** (nuvem) | Uso pelas cinco filiais, acessado por uma API |

## Evolução prevista

1. Hoje: aplicativo WPF acessando o banco diretamente (monólito).
2. Próximo passo: uma API ASP.NET Core entre o aplicativo e o banco na nuvem. Os contratos de cada módulo passam a ser usados pela API e pelo aplicativo.
3. Futuro: novo aplicativo em .NET MAUI (desktop, celular e web), usando a mesma API.