# LMFoods.Manager.Shared.Data

Infraestrutura de banco de dados compartilhada por todos os módulos (Acesso, Produtos...).
O banco é um só (`LMFoods_Banco`), e cada módulo tem suas tabelas no próprio schema (`acesso`, `produtos`...).

> ⚠️ Este projeto contém **apenas código técnico**: conexão, configuração e execução de scripts.
> Entidades, regras de negócio e scripts SQL ficam nos módulos. O `Shared.Data` não conhece nenhum módulo.

## Estrutura

```
LMFoods.Manager.Shared.Data/
├── DataOptions.cs            Configurações do banco (seção "Data" do appsettings.json)
├── SqlConnectionFactory.cs   Cria conexões abertas com o SQL Server
├── DatabaseMigrator.cs       Cria o banco e executa os scripts dos módulos (DbUp)
└── DependencyInjection.cs    Registra o DataOptions e a SqlConnectionFactory no app (AddSharedData)
```

## DataOptions

Lido da seção `"Data"` do `appsettings.json` do app (WPF ou, no futuro, API):

```json
"Data": {
  "Provedor": "Sql",
  "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=LMFoods_Banco;Trusted_Connection=True;TrustServerCertificate=True;",
  "IncluirDadosTeste": true
}
```

| Propriedade | Função | Padrão |
|---|---|---|
| `Provedor` | `Mock` (dados em memória) ou `Sql` (SQL Server) | `Mock` |
| `ConnectionString` | Obrigatória quando o provedor é `Sql` | vazia |
| `IncluirDadosTeste` | Executa os seeds (usuários de teste). Nunca ligar em produção | `false` |

O `Validar()` é chamado pelo `AddSharedData` na abertura do app. Ele recusa o provedor `Sql` sem connection string.

## SqlConnectionFactory

Os repositórios dos módulos recebem esta classe em vez da connection string, que fica num lugar só.
O método `AbrirAsync` devolve a conexão já aberta, e quem chama é responsável por fechá-la:

```csharp
await using var conexao = await _fabrica.AbrirAsync(ct);
```

## DatabaseMigrator

Cria o banco, se não existir, e executa os scripts embutidos nas DLLs dos módulos que ainda não rodaram:

```csharp
DatabaseMigrator.Migrar(connectionString, incluirDadosTeste, AcessoDataAssembly.Assembly);
```

- **Ordem:** os módulos rodam na ordem em que as DLLs são passadas. Dentro de cada módulo, pela numeração dos arquivos (`001_`, `002_`...).
- **Scripts e seeds:** arquivos da pasta `Scripts/` sempre rodam; os da pasta `Seeds/` só quando `incluirDadosTeste` é `true`.
- **Controle:** a tabela `dbo.SchemaVersions` registra os scripts executados. Cada script roda uma única vez.
- **Transação:** cada script roda numa transação própria. Se falhar, nada do que ele fez fica no banco.
- **Variáveis desligadas:** o DbUp trataria `$texto$` como variável e corromperia os hashes BCrypt dos seeds.
- **Síncrono:** no app, chamar fora da thread da interface: `await Task.Run(() => DatabaseMigrator.Migrar(...))`.

## Registro no app (AddSharedData)

O app registra o projeto com uma única chamada:

```csharp
services.AddSharedData(configuration);
```

- Lê a seção `"Data"` do `appsettings.json`. Sem a seção, usa os valores padrão (Mock, sem dados de teste).
- Chama o `Validar()`, para uma configuração errada impedir a abertura do app.
- Registra o `DataOptions` como instância única. Os módulos usam o `Provedor` dele para escolher entre os repositórios Mock e Sql.
- Registra a `SqlConnectionFactory` **apenas** com o provedor `Sql`.

## Como um módulo usa este projeto

1. O projeto `.Data` do módulo referencia o `LMFoods.Manager.Shared.Data`.
2. Os scripts ficam nas pastas `Scripts/` e `Seeds/` do módulo, embutidos na DLL pelo `.csproj`:

   ```xml
   <EmbeddedResource Include="Scripts\*.sql" />
   <EmbeddedResource Include="Seeds\*.sql" />
   ```

3. O módulo expõe a própria DLL por uma classe marcadora, como o `AcessoDataAssembly`:

   ```csharp
   public static class AcessoDataAssembly
   {
       public static Assembly Assembly { get; } = typeof(AcessoDataAssembly).Assembly;
   }
   ```

4. Os repositórios SQL do módulo recebem a `SqlConnectionFactory` pelo construtor.
5. O app passa as DLLs dos módulos para o `DatabaseMigrator`, na ordem de dependência.

## Pacotes

| Pacote | Função |
|---|---|
| `dbup-sqlserver` | Executa os scripts e registra os já executados |
| `Microsoft.Data.SqlClient` | Driver de conexão com o SQL Server (LocalDB, Express e Azure SQL) |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | Permite registrar as classes no DI do app (`IServiceCollection`) |
| `Microsoft.Extensions.Configuration.Binder` | Lê a seção `"Data"` do `appsettings.json` e preenche o `DataOptions` |
| `System.Security.Cryptography.Pkcs` | Dependência indireta do SqlClient, fixada numa versão mais nova para corrigir um alerta de segurança do NuGet. Ao atualizar o SqlClient, testar se ainda é necessária |

## Testes

Ficam em `LMFoods.Manager.Shared.Data.Tests` (xUnit v3):

| Pasta | O que testa |
|---|---|
| `Unit/` | Valores padrão e validação do `DataOptions`; validação dos parâmetros do `DatabaseMigrator` |
| `Integration/` | `SqlConnectionFactory` abrindo uma conexão real na LocalDB |

Os testes dos **scripts** de cada módulo ficam no projeto de testes do próprio módulo (ex.: `Acesso.Tests/Integration/ScriptsAcessoTests.cs`).