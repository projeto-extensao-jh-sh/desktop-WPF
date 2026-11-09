# Sistema Administrativo Multiplataforma — LM Food

Sistema administrativo para centralizar as operações da LM Food, permitindo que as cinco filiais compartilhem informações em um único ambiente.

## Sobre o projeto

Projeto de Extensão desenvolvido para um cliente real (LM Food), com o objetivo de unificar o controle de produtos, vendas e estoque entre as cinco filiais da empresa.

## 📦 Versões

### 📌 Versão atual: **v0.1.0**

👉 [🔽 Baixar última versão](https://github.com/OWNER/REPO/releases/latest)

- [Todas as versões](https://github.com/OWNER/REPO/releases)
- [Histórico de mudanças](CHANGELOG.md)

O versionamento segue o [SemVer](https://semver.org/lang/pt-BR/) (`MAJOR.MINOR.PATCH`). Versões de teste são publicadas como *pre-release*, com sufixo na tag (ex.: `v0.1.0-beta.1`), e não aparecem no link de última versão.

## Funcionalidades

- Login com usuário e PIN de 4 dígitos, com bloqueio após tentativas erradas
- Perfis de Administrador e Vendedor, com permissões distintas
- Cadastro de produtos (nome, categoria, preço, estoque)
- Registro de vendas por filial
- Compartilhamento em tempo real das informações entre as filiais
- Relatórios consolidados de vendas e estoque por unidade e por rede

## Tecnologias

- **Cliente:** WPF (.NET 10)
- **Banco de dados:** SQL Server LocalDB na primeira etapa
- **Próxima etapa:** migração do banco para Azure SQL Database (camada gratuita)
- **Etapa futura:** reconstrução do cliente em .NET MAUI (desktop, Android/iOS e web via Blazor Hybrid), consumindo os dados por uma API ASP.NET Core hospedada no Azure App Service

## Estrutura da solução

A solução é organizada por **módulos de negócio**. Cada módulo fica numa pasta própria (por exemplo, `Acesso`), e o código técnico usado por todos os módulos fica na pasta `Shared`. Todas as telas ficam no projeto WPF.

Os projetos seguem o padrão de nome `LMFoods.Manager.<Módulo>`, e cada pasta de projeto tem o mesmo nome do projeto.

Estrutura atual:

```
lmfoods-manager/
├── LMFoods.Manager.slnx
├── README.md
├── .gitignore
├── docs/
│   └── arquitetura.md
│
├── LMFoods.Manager.Wpf/
│   ├── App.xaml
│   ├── LMFoodsStyles.xaml           ← cores, fontes e estilos do sistema
│   ├── Commands/
│   │   └── RelayCommand.cs
│   ├── ViewModels/
│   │   ├── ViewModelBase.cs
│   │   ├── LoginViewModel.cs
│   │   └── StartViewModel.cs
│   ├── Views/
│   │   ├── LoginView.xaml
│   │   ├── LoginPinView.xaml
│   │   ├── CadastroUsuarioView.xaml
│   │   ├── PanelControlView.xaml
│   └── Resources/                   ← logo, ícone e imagens dos pratos
│
├── Shared/
│   ├── LMFoods.Manager.Shared.Data/
│   └── LMFoods.Manager.Shared.Data.Tests/
│
└── Acesso/
    ├── LMFoods.Manager.Acesso.Contracts/
    ├── LMFoods.Manager.Acesso/
    ├── LMFoods.Manager.Acesso.Data/
    └── LMFoods.Manager.Acesso.Tests/
```

A arquitetura completa (divisão em módulos, projetos de cada módulo e direção das dependências) está em [docs/arquitetura.md](docs/arquitetura.md).

### `LMFoods.Manager.Wpf`
**Tipo:** WPF Application (`net10.0-windows`)

Aplicativo desktop com as telas do sistema, organizado no padrão MVVM (telas em `Views`, a lógica de cada tela em `ViewModels` e as ações dos botões com o `RelayCommand`). As cores e os estilos aprovados pelo cliente ficam no `LMFoodsStyles.xaml`.

Telas já criadas:

| Tela | Função |
|---|---|
| `LoginView` | Seleção do usuário que vai entrar |
| `LoginPinView` | Digitação do PIN de 4 dígitos |
| `CadastroUsuarioView` | Cadastro de usuários |
| `PanelControlView` | Painel de controle (tela principal) |
| `VentasView` | Vendas |

É o ponto de entrada do sistema. As telas vão usar apenas as interfaces e os DTOs dos projetos `Contracts` de cada módulo, sem acessar o banco de dados diretamente. Por enquanto, as telas ainda não estão ligadas ao banco: isso acontece na etapa de integração com o módulo Acesso.

### Shared

Código técnico usado por todos os módulos. Não contém regras de negócio nem conhece nenhum módulo.

#### `LMFoods.Manager.Shared.Data`
**Tipo:** Class Library (`net10.0`)

Infraestrutura de banco compartilhada: configurações do banco (`DataOptions`), criação de conexões (`SqlConnectionFactory`), execução dos scripts SQL dos módulos (`DatabaseMigrator`, com DbUp) e o registro no app (`AddSharedData`). O banco é um só (`LMFoods_Banco`), e cada módulo tem suas tabelas no próprio schema.

#### `LMFoods.Manager.Shared.Data.Tests`
**Tipo:** xUnit Test Project (`net10.0`)

Testes da infraestrutura compartilhada: validação das configurações, parâmetros do migrador e abertura de conexão na LocalDB.

### Módulo Acesso

Responsável pelo login com PIN, cadastro e edição de usuários, além das filiais e caixas a que cada usuário pertence e do registro das sessões (login e logout).

#### `LMFoods.Manager.Acesso.Contracts`
**Tipo:** Class Library (`net10.0`)

Define o que o módulo oferece para o resto do sistema: interfaces dos serviços (como `IAuthService`), DTOs e objetos de requisição. É o único projeto do módulo que as telas conhecem. Quando o sistema passar a usar uma API, esses mesmos contratos serão usados tanto pela API quanto pelo cliente.

#### `LMFoods.Manager.Acesso`
**Tipo:** Class Library (`net10.0`)

Contém as regras de negócio do módulo: entidades (`Filial`, `Caixa`, `Usuario`, `SessaoUsuario`), validações, verificação do PIN com hash BCrypt, bloqueio por tentativas e as implementações dos serviços definidos em `Contracts`. Também define as interfaces dos repositórios, mas não sabe qual banco de dados está sendo usado.

#### `LMFoods.Manager.Acesso.Data`
**Tipo:** Class Library (`net10.0`)

Camada de acesso a dados do módulo. Implementa os repositórios para SQL Server e as versões Mock (dados em memória). Também guarda os scripts SQL de criação das tabelas e de dados de teste, embutidos na DLL e executados pelo `DatabaseMigrator` do `Shared.Data` na inicialização. A troca entre Mock, SQL Server local e Azure SQL é feita apenas pela configuração (`appsettings.json`).

#### `LMFoods.Manager.Acesso.Tests`
**Tipo:** xUnit Test Project (`net10.0`)

Testes automatizados do módulo. Os testes de unidade validam as regras de negócio usando os repositórios Mock. Os testes de integração validam os scripts SQL e os repositórios contra um banco LocalDB temporário, criado e apagado a cada teste.

### Referências entre projetos

```
Shared.Data        ← não depende de nenhum projeto
Acesso.Contracts   ← não depende de nenhum projeto
Acesso             → Acesso.Contracts
Acesso.Data        → Acesso, Acesso.Contracts, Shared.Data
Acesso.Tests       → Acesso.Contracts, Acesso, Acesso.Data, Shared.Data
Wpf                → Acesso.Contracts, Acesso, Acesso.Data, Shared.Data
```

As regras de negócio (`Acesso`) não dependem do acesso a dados (`Acesso.Data`). Isso permite trocar o banco de dados, ou mover essa camada para trás de uma API, sem alterar as regras. O `Shared.Data` não depende de nenhum módulo, e cada referência é adicionada apenas quando passa a ser usada.

### Novos módulos

Os próximos módulos (Produtos, Vendas, Relatórios) seguem o mesmo padrão: uma pasta por módulo, com os projetos `LMFoods.Manager.<Módulo>`, `LMFoods.Manager.<Módulo>.Data` e, quando necessário, `.Contracts` e `.Tests`. O projeto `.Data` de cada módulo referencia o `Shared.Data` e expõe seus scripts por uma classe marcadora (como o `AcessoDataAssembly`).

## Plano de ação

✅ concluído · 🔄 em andamento · ❌ não iniciado

1. ✅ Levantamento de requisitos junto à LM Food
2. 🔄 Desenvolvimento do app em WPF
3. ❌ Teste com um usuário e banco em memória
4. 🔄 Modelagem do banco de dados (produtos, usuários, vendas, filiais)
5. ❌ Teste com 1 usuário + SQL Server local
6. ❌ Teste com 2 usuários + SQL Server em rede (rede simulada para testar múltiplos acessos)
7. ❌ Migração do banco para Azure SQL Database
8. ❌ Testes com uma filial piloto antes do rollout completo

## Equipe

- Professor (Orientador): Dácio Moreira de Souza
- Samuel Hermany
- Johanna Liliana Liza Herrera