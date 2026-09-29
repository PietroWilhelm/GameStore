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

---

# 🚦 CP5 — Versionamento de API, Paginação e Rate Limit

Evolução do contrato HTTP de **Game** (recurso já entregue no CP3), sem quebrar quem ainda consome o contrato antigo, mais um limite de taxa em um endpoint de escrita. Nenhum outro recurso do CP1–CP4 foi removido: `DbContext`, migrations, controllers, DTOs, Swagger, `IRepository<T>`, `GlobalExceptionHandler`, `GET /health`, os logs com `TraceId` e os projetos `GameStore.Domain.Tests`/`GameStore.Application.Tests` continuam intactos.

## Como subir a API

Igual ao CP4 (nenhuma mudança no fluxo de build/migração):

```bash
dotnet restore GameStore.sln
dotnet build GameStore.sln
dotnet ef database update --project GameStore.Infrastructure --startup-project GameStore.Api
dotnet run --project GameStore.Api
```

A partir daqui, `<host>` = `http://localhost:<porta>` (a porta exata está em `GameStore.Api/Properties/launchSettings.json`).

## URLs relevantes

| O que | URL |
|---|---|
| Swagger UI (ambiente Development) | `<host>/` |
| Swagger JSON v1 (obsoleta) | `<host>/swagger/v1/swagger.json` |
| Swagger JSON v2 (atual) | `<host>/swagger/v2/swagger.json` |
| Health check | `<host>/health` |
| Listagem de games — v1 (obsoleta, sem paginação) | `<host>/api/game` (com versão v1, ver abaixo) |
| Listagem de games — v2 (paginada, padrão quando a versão não é informada) | `<host>/api/game` ou `<host>/api/v2/game` |

## A) Versionamento de API

Recurso escolhido: **Game** (`GameController`). Duas versões convivem lado a lado, compartilhando o mesmo `IGameService`/`GameService` — nenhuma regra de negócio é duplicada entre elas:

* **v1.0** — marcada como obsoleta (`[ApiVersion("1.0", Deprecated = true)]`). `GET` de listagem mantém o contrato antigo: um array simples de games, sem paginação. Preservada apenas por compatibilidade.
* **v2.0** — versão atual e **padrão** quando nenhuma versão é informada (`DefaultApiVersion = 2.0`, `AssumeDefaultVersionWhenUnspecified = true`). `GET` de listagem retorna o envelope paginado (seção B).

`GetById`, `POST` (criação) e `DELETE` não têm atributo de versão específico — por isso respondem em **ambas** as versões, sem duplicação de código.

Os outros quatro recursos (`Studio`, `Genre`, `Customer`, `Order`) são `[ApiVersionNeutral]`: continuam funcionando exatamente como antes e continuam aparecendo no Swagger de **cada** versão — versionar `Game` não os removeu do contrato nem da documentação.

**Como especificar a versão** (qualquer uma das três formas funciona; se nenhuma for informada, a API assume v2.0):

```bash
# 1) Query string
curl "<host>/api/game?api-version=1.0"
curl "<host>/api/game?api-version=2.0"

# 2) Header
curl -H "X-Api-Version: 1.0" "<host>/api/game"
curl -H "X-Api-Version: 2.0" "<host>/api/game"

# 3) Segmento de URL (adição recomendada, não substitui as anteriores)
curl "<host>/api/v1/game"
curl "<host>/api/v2/game"
```

Toda resposta inclui os headers `api-supported-versions` e `api-deprecated-versions` (`ReportApiVersions = true`), permitindo a um cliente descobrir programaticamente que a v1.0 está obsoleta.

O Swagger (`<host>/`) lista dois grupos de documentação — **v1** (com aviso de obsolescência na descrição) e **v2** — cada um mostrando os endpoints de `Game` daquela versão mais todos os recursos version-neutral.

Cada action de `Game` responde em duas rotas equivalentes (`api/game`, selecionada por query string/header, e `api/v{version}/game`, pelo segmento de URL) — para o Swagger não ficar com cada operação listada em dobro, só a rota sem segmento de versão é documentada (`DocInclusionPredicate` em `SwaggerServiceCollectionExtensions.cs`); a rota com segmento continua funcionando normalmente, só não aparece separadamente na documentação.

## B) Paginação (somente v2)

A v1 de `GET /api/game` **não paginou** — segue devolvendo a lista completa, exatamente como no CP3/CP4 (sem quebra de contrato). Apenas a v2 pagina.

**Parâmetros** (query string, em `GET <host>/api/v2/game` ou `GET <host>/api/game?api-version=2.0`):

| Parâmetro | Default | Regra |
|---|---|---|
| `page` | `1` | Inteiro ≥ 1 |
| `pageSize` | `20` | Inteiro entre 1 e 100 |

* `page`/`pageSize` fora do intervalo permitido → `400 Bad Request` (`application/problem+json`, via `GlobalExceptionHandler`, que já mapeia `ArgumentException`).
* `page` além do total de páginas existentes → `200 OK` com `items: []` (nunca `404`).

**Envelope de resposta** (nomes de campo exatos):

```json
{
  "page": 1,
  "pageSize": 20,
  "totalItems": 57,
  "totalPages": 3,
  "items": [ { "id": "...", "name": "..." } ],
  "hasPrevious": false,
  "hasNext": true
}
```

`totalPages` = `ceiling(totalItems / pageSize)`. `hasPrevious`/`hasNext` são um extra sobre o contrato mínimo pedido.

**Camadas envolvidas** (Clean Architecture, sem paginação em memória):

* `GameController.GetAllV2` (Api) — só lê `page`/`pageSize` da query string.
* `GameService.GetPaged` (Application) — valida o intervalo (lança `ArgumentException` se inválido) e monta o `PagedResponse<GameResponse>` (`GameStore.Application/DTOs/PagedResponse.cs`).
* `Repository<T>.GetPaged` (Infrastructure, `GameStore.Infrastructure/Repositories/Repository.cs`) — `Count()` + `OrderBy(x => x.CreatedAt)` + `Skip` + `Take`, tudo avaliado como `IQueryable<T>` e traduzido para SQL pelo EF Core; o `ToList()` final já materializa só os itens da página. `GetById` nunca pagina.

## C) Rate Limiting

Middleware nativo `Microsoft.AspNetCore.RateLimiting` (nenhum pacote de terceiros). Nenhuma política é global — só os endpoints marcados com `[EnableRateLimiting(...)]` são afetados, e `GET /health` não tem esse atributo, portanto **nunca** passa por limite de taxa.

| Política | Endpoint | Limite | Janela | Partição |
|---|---|---|---|---|
| `write-fixed` (obrigatória) | `POST /api/game` (criação) | 10 requisições | 1 minuto (fixed window) | por IP de origem |
| `read-fixed` (extra) | `GET` v2 de `/api/game` (listagem paginada) | 60 requisições | 1 minuto (fixed window) | por IP de origem |

Ao exceder o limite: `429 Too Many Requests`, com header `Retry-After` (em segundos) e corpo JSON (`application/problem+json`):

```json
{
  "type": "about:blank",
  "title": "Limite de requisições excedido",
  "status": 429,
  "detail": "Você excedeu o limite de requisições para este endpoint. Tente novamente em 60 segundo(s).",
  "instance": "/api/game"
}
```

**Como testar sem script** (Windows PowerShell — repetir manualmente ou colar 11+ vezes em poucos segundos):

```powershell
for ($i = 1; $i -le 11; $i++) {
  curl.exe -s -o NUL -w "tentativa $i -> %{http_code}`n" -X POST "<host>/api/game" -H "Content-Type: application/json" -d "{\"name\":\"Teste $i\",\"description\":\"Descricao valida de teste\",\"launchDate\":\"2020-01-01\",\"studioId\":\"<guid-de-um-studio-existente>\",\"contentTypeEnum\":0}"
}
```

A 11ª chamada dentro do mesmo minuto deve retornar `429`. Logo depois, `GET <host>/health` deve continuar respondendo `200` normalmente (prova de que o rate limit não afetou o health check).

Middleware registrado em `Program.cs` entre `UseExceptionHandler()` e `MapControllers()` (`app.UseRateLimiter()`), conforme exigido.

## Como rodar os testes

Sem alteração no comando (a partir da pasta `GameStore`, onde está o `.sln`):

```bash
dotnet test GameStore.sln
```

Testes do CP4 continuam passando sem alteração. Novos testes do CP5 em `GameStore.Application.Tests/Services/Implementations/GameServiceTests.cs` (sem API nem banco):

* `GetPaged_ComPageOuPageSizeInvalidos_DeveLancarArgumentException` — `[Theory]`/`[InlineData]` cobrindo `page`/`pageSize` fora do intervalo (`page <= 0`, `pageSize <= 0`, `pageSize > 100`); confirma `400` (via `ArgumentException`, mapeada pelo `GlobalExceptionHandler`) e que o repositório nunca é chamado.
* `GetPaged_ComPageEPageSizeValidos_DeveRetornarEnvelopePaginadoComTotalPagesCorreto` — `[Fact]` cobrindo o caminho feliz: `totalPages`, `hasPrevious`/`hasNext` e delegação ao repositório com os parâmetros corretos.

## Tabela de mapeamento de exceções

Sem alterações desde o CP4 — ver a tabela na seção "🩺 CP4 — Health Checks, Observabilidade e Testes" acima. A validação de paginação reaproveita o mapeamento existente de `ArgumentException` → `400`.

## 📁 `/docs` — evidências do CP5

Além do que já existe do CP1–CP4, adicionar em `/docs`:

* JSON da listagem v1 (`GET /api/game?api-version=1.0`) e da v2 (`GET /api/game?api-version=2.0`) do **mesmo recurso**, lado a lado.
* Print/trecho dos headers de resposta mostrando `api-supported-versions` e `api-deprecated-versions`.
* Print do Swagger (`<host>/`) mostrando os dois grupos de documentação (v1 com aviso de obsoleta, v2).
* Print/trecho de um `400 Bad Request` com `page`/`pageSize` inválidos (ex.: `pageSize=0` ou `pageSize=500`).
* Print/trecho comparando página 1 e página 2 da listagem v2 (mesmo `pageSize`, `items` diferentes, `totalItems`/`totalPages` coerentes).
* Print/trecho de um `429 Too Many Requests` com o header `Retry-After`, seguido de um `GET /health` respondendo `200` imediatamente depois (prova de que `/health` não foi afetado).
* Saída de `dotnet test` (ou print do Test Explorer) com todos os testes — CP4 + CP5 — passando.
