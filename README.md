# 🎮 GameStore  
## 👥 Integrantes do Grupo

Pietro Paranhos Wilhelm Rm561378

João Vitor Biribilli Ravelli Rm565594

Gabriel Neris Losano Rm564093 

## 🗂️ Entidades Modeladas
Content: Classe base que contém atributos comuns como Nome, Descrição e Data de Lançamento.

Game: Entidade especializada que herda de Content.
Studio: Representa a empresa desenvolvedora do jogo.

Category: Define os gêneros dos jogos (Ex: RPG, Ação, Terror).

Customer: Entidade principal do usuário, contendo dados cadastrais, CPF, e-mail e validações de idade.

CustomerConfiguration: Armazena preferências do usuário, como tema da interface.

## 🎯 Domínio Escolhido
O domínio selecionado para este projeto é uma Loja de Jogos (GameStore). O sistema foca no gerenciamento do catálogo de produtos (jogos), seus respectivos desenvolvedores (studios), categorização e a gestão de perfis de clientes com suas configurações personalizadas.


## 🔗 Resumo dos Relacionamentos

Studio 1 : N Game: Um Studio pode desenvolver múltiplos jogos, mas cada jogo é associado a um único Studio principal.

Game N : N Category: Um jogo pode pertencer a várias categorias, e uma categoria pode estar vinculada a diversos jogos (resolvido via tabela associativa).

Customer 1 : N Order: Um cliente pode realizar vários pedidos ao longo do tempo.

Game N : N : Um pedido pode conter vários jogos e um jogo pode estar em vários pedidos diferentes.

## 📋 Funcionamento do projeto e documentação
Os prints mostrando o funcionamento do projeto está em docs, contém imagens do migrations e a criação dentro do banco 
 
## 🔖 Banco utilizado
O banco que foi utilizado para o projeto foi OracleSql 

## 📍 Sequência de comandos para Migration
Rodar a partir da pasta onde vc baixou o projeto exemplo:

E:\2TDSPG\Developement with .net\GameStore\GameStore
 
1) Restaurar os pacotes

dotnet restore GameStore.sln

2) Compilar a solução

dotnet build GameStore.sln

3) Criar uma nova migration

Exemplo com o nome InitialCreate:

dotnet ef migrations add InitialCreate --project GameStore.Infrastructure --startup-project GameStore.Api

Se for outra migration, trocar o nome:

dotnet ef migrations add AddStudioRelation --project GameStore.Infrastructure --startup-project GameStore.Api
 
4) Listar as migrations existentes

dotnet ef migrations list --project GameStore.Infrastructure --startup-project GameStore.Api

5) Aplicar a migration no banco

dotnet ef database update --project GameStore.Infrastructure --startup-project GameStore.Api

# ❗ Observação importante no projeto

No estado atual do meu projeto, o Oracle já mostrou que ao gerar migration automaticamente ele pode criar tipos como:
NVARCHAR2
BOOLEAN
E no meu banco isso causou erro.
Então, depois de gerar a migration, a você precisa revisar o arquivo em GameStore.Infrastructure\Migrations\... e garantir que fique assim:

✅ NVARCHAR2 → VARCHAR2

✅ BOOLEAN → CHAR(1)

🔴 Senão o database update pode falhar.

---

# 🩺 CP4 — Health Checks, Observabilidade e Testes

## Como subir a API

A partir da pasta `GameStore` (onde está o `.sln`):

```bash
dotnet restore GameStore.sln
dotnet build GameStore.sln
dotnet ef database update --project GameStore.Infrastructure --startup-project GameStore.Api
dotnet run --project GameStore.Api
```

URLs (ambiente Development):

* Swagger: `http://localhost:<porta>/` (raiz — `RoutePrefix` configurado como `""`)
* Health check: `http://localhost:<porta>/health`

A porta exata está em `GameStore.Api/Properties/launchSettings.json`.

## `GET /health`

Único endpoint de health check, expõe um relatório JSON com o status agregado, a duração total e o detalhe de cada check. Registrado em `HealthCheckServiceCollectionExtensions.AddGameStoreHealthChecks` (`GameStore.Api/Extensions/`) para não inchar o `Program.cs`.

Checks registrados:

| Nome | O que verifica | Implementação |
|---|---|---|
| `self` | O processo da API está no ar (sempre `Healthy`, não depende de nada externo). | `HealthCheckResult.Healthy(...)` inline |
| `oracle` | Conectividade com o banco Oracle usado pela API, via `GameStoreContext.Database.CanConnectAsync()`. | `GameStore.Api/Health/GameStoreDbHealthCheck.cs` |
| `fiap-site` | Dependência externa opcional (recomendado no enunciado): disponibilidade de `https://www.fiap.com.br`. | `GameStore.Api/Health/FiapSiteHealthCheck.cs` |

Abordagem escolhida para o check de banco: **check próprio** (`IHealthCheck` chamando `CanConnectAsync`) em vez do pacote `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` (`AddDbContextCheck<TContext>`) — funcionalmente equivalente, mas sem adicionar mais uma dependência NuGet ao projeto.

Resposta JSON (via `HealthCheckResponseWriter`, não é o texto `Healthy` padrão):

```json
{
  "status": "Healthy",
  "totalDurationMs": 42.1,
  "checks": [
    { "name": "self", "status": "Healthy", "description": "O processo da API está no ar.", "durationMs": 0.01, "error": null },
    { "name": "oracle", "status": "Healthy", "description": "Banco de dados acessível.", "durationMs": 38.7, "error": null },
    { "name": "fiap-site", "status": "Healthy", "description": "https://www.fiap.com.br respondeu 200.", "durationMs": 120.4, "error": null }
  ]
}
```

O detalhe de exceção (`error`) só é preenchido em ambiente Development — em Production fica `null`, mesmo que o check falhe.

Status HTTP:

* `Healthy` → `200`
* `Degraded` → `200` (ainda serve tráfego, com aviso)
* `Unhealthy` → `503`

Como qualquer check `Unhealthy` derruba o status agregado do relatório inteiro, uma falha no `oracle` (banco fora do ar) ou no `fiap-site` (site externo indisponível) faz `/health` responder `503`, mesmo que o processo (`self`) continue saudável.

**Como simular falha**: parar o banco Oracle, ou trocar a connection string em `appsettings.Development.json` por um valor inválido antes de subir a API — o check `oracle` deve virar `Unhealthy` e `/health` deve responder `503`.

## Logs / Observabilidade

* `ILogger<T>` nativo do ASP.NET Core em toda a aplicação.
* No `GameController.Create` (fluxo de escrita): log de início (`Iniciando criação de game...`) e de sucesso (`Game criado com sucesso...`), com propriedades nomeadas (`{GameName}`, `{StudioId}`, `{GameId}`, `{TraceId}`) e correlação via `HttpContext.TraceIdentifier`.
* No `GlobalExceptionHandler` (`GameStore.Api/Exceptions/`): toda exceção não tratada é logada em nível `Error`, incluindo o mesmo `TraceId` da requisição.
* Em Development, o `traceId` também é incluído em `ProblemDetails.Extensions`, para facilitar correlacionar a resposta HTTP com a linha de log correspondente.
* Em Production, a resposta HTTP nunca inclui stack trace — o `GlobalExceptionHandler` sempre devolve uma mensagem amigável; o detalhe completo fica só no log do servidor.
* `/health` não aparece no Swagger (não é uma action de controller, então o Swashbuckle não o lista).

## Tabela de mapeamento de exceções (`GlobalExceptionHandler`)

| Exceção | Status HTTP | Título |
|---|---|---|
| `ArgumentNullException` / `ArgumentException` | 400 | Requisição inválida |
| `CreateException` (regra de negócio de criação) | 400 | Não foi possível concluir a operação |
| `DomainException` | 400 | Não foi possível concluir a operação |
| `InvalidOperationException` | 400 | Não foi possível concluir a operação |
| `KeyNotFoundException` | 404 | Recurso não encontrado |
| `UnauthorizedAccessException` | 401 | Não autorizado |
| `DbException` (falha de conexão/consulta ao banco) | 502 | Banco indisponível |
| Qualquer outra exceção | 500 | Erro interno do servidor |

## Como rodar os testes

A partir da pasta `GameStore` (onde está o `.sln`):

```bash
dotnet test GameStore.sln
```

Projetos de teste (xUnit):

* `GameStore.Domain.Tests` — testa regras de negócio das entidades de domínio sem mock (`Game`, `Customer`), com `[Fact]` no caminho feliz e `[Theory]`/`[InlineData]` no caminho de erro (ex.: cliente com menos de 13 anos).
* `GameStore.Application.Tests` — testa `GameService` com mock (Moq) de `IGameRepository`/`IStudioRepository`: dependência ausente (nome duplicado, studio inexistente) lança `CreateException` e não chama `Add` (`Times.Never`); caminho feliz persiste uma vez (`Times.Once`).

## 📁 `/docs`

Além do material já existente (MER e prints do CP1–3), adicionar aqui, como evidência do CP4:

* Print ou trecho JSON de `/health` com status `Healthy`.
* Print ou trecho de `/health` com status `Unhealthy` (banco parado, ou connection string inválida local).
* Trecho de log (console) de um `POST /api/game` mostrando o `TraceId`, e/ou de uma exceção tratada pelo `GlobalExceptionHandler`.
* Saída de `dotnet test` (ou print do Test Explorer) com todos os testes passando.
