# Sistema Administrativo Multiplataforma — LM Food

Sistema administrativo para centralizar as operações da LM Food, permitindo que as cinco filiais compartilhem informações em um único ambiente.

## Sobre o projeto

Projeto de Extensão desenvolvido para um cliente real (LM Food), com o objetivo de unificar o controle de produtos, vendas e estoque entre as cinco filiais da empresa.

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
└── LMFoods.Manager.Wpf/
    ├── App.xaml
    ├── LMFoodsStyles.xaml           ← cores, fontes e estilos do sistema
    ├── Commands/
    │   └── RelayCommand.cs
    ├── ViewModels/
    │   ├── ViewModelBase.cs
    │   ├── LoginViewModel.cs
    │   └── StartViewModel.cs
    ├── Views/
    │   ├── LoginView.xaml
    │   ├── LoginPinView.xaml
    │   ├── CadastroUsuarioView.xaml
    │   ├── PanelControlView.xaml
    │   └── VentasView.xaml
    └── Resources/                   ← logo, ícone e imagens dos pratos
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